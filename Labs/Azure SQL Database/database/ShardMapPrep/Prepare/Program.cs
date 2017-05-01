using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Azure.SqlDatabase.ElasticScale;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using Newtonsoft.Json;

namespace Prepare
{
    public class Program
    {
        private const int ConnectionTimeoutSeconds = 60;

        private const string ShardServer = "<server>.database.windows.net";
        private const string ShardMapDatabase = "Head";
        private const string ShardDatabasePoolName = "ShardDatabasePool";

        private const string Username = "<username>";
        private const string Password = "<password>";

        public static int FirstShardId = 1;
        public static int LastShardId = 91;

        public static void Main()
        {
            try
            {
                TryCreateShardMap();

                Console.WriteLine("Done.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:");
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(intercept: true);
        }

        private static void TryCreateShardMap()
        {
            var connectionString = new SqlConnectionStringBuilder
            {
                ConnectTimeout = ConnectionTimeoutSeconds,
                DataSource = ShardServer,
                InitialCatalog = ShardMapDatabase,
                UserID = Username,
                Password = Password
            }
            .ToString();

            var shardConnectionString = new SqlConnectionStringBuilder
            {
                ConnectTimeout = ConnectionTimeoutSeconds,
                UserID = Username,
                Password = Password
            }
            .ToString();

            var shardMapManager = GetOrCreateCreateSqlShardMapManager(connectionString);

            var map = GetOrCreateListShardMap<int>(shardMapManager, "OrderShardMap");
            
            Console.WriteLine("Creating shards");
            BuildAndPrepareShards(map, connectionString, shardConnectionString);
        }

        private static void BuildAndPrepareShards(ListShardMap<int> shardMap, string headConnectionString, string shardConnectionString)
        {
            // Create order shards
            for (var customerId = FirstShardId; customerId <= LastShardId; customerId++)
            {
                // Retry each shard in a loop
                bool retrying = true;
                while (retrying)
                {
                    try
                    {
                        var shard = GetOrCreateOrderShard(shardMap, headConnectionString, customerId);

                        PrepareOrderShard(shard, customerId, shardConnectionString);

                        // Break out of the retry loop
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error on shard {customerId} ({ex.GetType()})\n{ex.Message}");
                    }


                    switch (ReadContinueAction())
                    {
                        case ContinueAction.Retry:
                        {
                            Console.WriteLine("Retrying...");
                            break;
                        }
                        case ContinueAction.Skip:
                        {
                            Console.WriteLine("Skipping...");
                            retrying = false;
                            break;
                        }
                        case ContinueAction.Halt:
                        {
                            Console.WriteLine("Halting...");
                            return;
                        }
                    }
                }
            }
        }

        private static ContinueAction ReadContinueAction()
        {
            while (true)
            {
                Console.WriteLine("Press R to retry, S to skip, Q or ESC to exit");
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.R)
                {
                    return ContinueAction.Retry;
                }
                if (key.Key == ConsoleKey.S)
                {
                    return ContinueAction.Skip;
                }
                if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                {
                    return ContinueAction.Halt;
                }
            }
        }

        private static Shard GetOrCreateOrderShard(ListShardMap<int> shardMap, string headConnectionString, int customerId)
        {
            string databaseName = $"CustomerOrders{customerId}";

            var shardLocation = new ShardLocation(ShardServer, databaseName);

            // Does this shard already exist?
            Shard shard;
            if (shardMap.TryGetShard(shardLocation, out shard))
            {
                Console.WriteLine($"Found order shard at {databaseName}");
                return shard;
            }

            // Create the database
            try
            {
                using (var headConn = new SqlConnection(headConnectionString))
                {
                    headConn.Open();

                    Console.WriteLine($"Creating order shard database {databaseName}");
                    headConn.Execute(sql: $@"create database {databaseName} ( SERVICE_OBJECTIVE = ELASTIC_POOL(name = {ShardDatabasePoolName}) )", commandTimeout: 0);
                }
            }
            catch(SqlException ex)
            {
                // Does the database already exist?
                if (ex.Message.Contains("already exists."))
                {
                    Console.WriteLine("Database already exists");
                }
                else throw;
            }

            // Add to the shard map
            Console.WriteLine($"Adding {databaseName} to Shard Map");
            return shardMap.CreateShard(shardLocation);
        }

        private static void PrepareOrderShard(Shard shard, int customerId, string shardConnectionString)
        {
            Console.WriteLine($"Connecting to shard {shard.Location.Database}");
            
            using (var conn = shard.OpenConnection(shardConnectionString))
            {
                // Clear order and order details tables
                Console.WriteLine("Dropping tables");

                conn.Execute(sql: "if (exists (select 1 from sys.tables where name = 'Orders')) begin " +
                                  "drop table dbo.[Order Details]; " +
                                  "drop table dbo.[Orders]; " +
                                  "end", commandTimeout: 0);

                // Prepare the database
                Console.Write($"Preparing shard {shard.Location.Database}: ");

                var prepareStatements = File.ReadAllText("3_order_shard_prepare.sql").Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var sql in prepareStatements)
                {
                    conn.Execute(sql: sql, commandTimeout: 0);
                    Console.Write("#");
                }

                Console.WriteLine();

                // Insert orders
                Console.WriteLine($"Inserting orders for customer {customerId}");
                const string INSERT_ORDER_SQL = "insert into dbo.[Orders] (CustomerID, OrderID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight) " + "values (@CustomerID, @OrderID, @EmployeeID, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, @Freight)";
                var ordersFilteredByCustomerId = ReadJsonFromFile<OrderRecord>("orders.json").Where(o => o.CustomerID == customerId).ToArray();
                conn.Execute(sql: INSERT_ORDER_SQL, param: ordersFilteredByCustomerId, commandTimeout: 0);

                // Insert order details
                Console.WriteLine($"Inserting order details for customer {customerId}");
                const string INSERT_ORDER_DETAIL_SQL = "insert into dbo.[Order Details] (CustomerID, OrderID, ProductID, UnitPrice, Quantity, Discount) " + "values (@CustomerID, @OrderID, @ProductID, @UnitPrice, @Quantity, @Discount)";
                var detailsFilteredByCustomerId = ReadJsonFromFile<OrderDetailRecord>("order details.json").Where(o => o.CustomerID == customerId).ToArray();
                conn.Execute(sql: INSERT_ORDER_DETAIL_SQL, param: detailsFilteredByCustomerId, commandTimeout: 0);
            }
        }

        private static IList<T> ReadJsonFromFile<T>(string filename)
        {
            using (var file = File.OpenRead(filename))
            using (var reader = new StreamReader(file))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return new JsonSerializer().Deserialize<IList<T>>(jsonReader);
            }
        }

        private static ShardMapManager GetOrCreateCreateSqlShardMapManager(string connectionString)
        {
            ShardMapManager manager;
            if (ShardMapManagerFactory.TryGetSqlShardMapManager(connectionString, ShardMapManagerLoadPolicy.Eager, RetryBehavior.DefaultRetryBehavior, out manager))
            {
                Console.WriteLine("Shard map manager already exists");
                return manager;
            }

            Console.WriteLine("Creating shard map manager");
            return ShardMapManagerFactory.CreateSqlShardMapManager(connectionString, ShardMapManagerCreateMode.KeepExisting, RetryBehavior.DefaultRetryBehavior);
        }

        private static ListShardMap<T> GetOrCreateListShardMap<T>(ShardMapManager shardMapManager, string shardMapName)
        {
            ListShardMap<T> map;
            if (shardMapManager.TryGetListShardMap(shardMapName, out map))
            {
                // Shard map already exists
                Console.WriteLine("Shard map {0} already exists", shardMapName);
                return map;
            }

            // Initialize shard map
            Console.WriteLine("Creating shard map {0}", shardMapName);
            return shardMapManager.CreateListShardMap<T>(shardMapName);
        }
    }

    internal enum ContinueAction
    {
        Retry,
        Skip,
        Halt
    }
}

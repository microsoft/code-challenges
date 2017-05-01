using System;
using System.Web.Mvc;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;

namespace LabWeb.Controllers
{
    public class Scenario3Controller : Controller
    {
        private static ListShardMap<int> GetOrderShardMap()
        {
            var headConnectionString = Config.GetHeadDatabaseConnectionString();

            // The shard map is hosted on the "Head" database
            var manager = ShardMapManagerFactory.GetSqlShardMapManager(headConnectionString, ShardMapManagerLoadPolicy.Eager);

            // There shard map is identified by name
            var shardMap = manager.GetListShardMap<int>("OrderShardMap");

            return shardMap;
        }

        /// <summary>
        /// The Order shard map.
        /// The shard map is thread safe and should be cached.
        /// </summary>
        private static readonly Lazy<ListShardMap<int>> OrderShardMap = new Lazy<ListShardMap<int>>(GetOrderShardMap);

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult CreateOrders()
        {
            var shardMap = OrderShardMap.Value;

            var shardConnectionString = Config.GetOrderShardConnectionString();

            // Pick two customers whose orders fall into different shards
            var firstCustomerId = 22;
            var secondCustomerId = 87;

            int recordsChanged = 0;
            
            // OPEN TRANSACTION HERE
            {
                // We determine which shard to connect to by passing in the shard key
                // In this case Orders are sharded by CustomerId
                using (var conn = shardMap.OpenConnectionForKey(firstCustomerId, shardConnectionString))
                {
                    // Insert a new order!
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = CreateOrderSql(firstCustomerId);
                        recordsChanged += cmd.ExecuteNonQuery();
                    }
                }

                // Locate the second shard with the second customer Id
                using (var conn = shardMap.OpenConnectionForKey(secondCustomerId, shardConnectionString))
                {
                    // Insert another order!
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = CreateOrderSql(secondCustomerId);
                        recordsChanged += cmd.ExecuteNonQuery();
                    }
                }

                // COMMIT TRANSACTION HERE
            }

            TempData["RecordsChanged"] = recordsChanged;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Generates SQL for creating an arbitrary Order for the given customerId
        /// </summary>
        private static string CreateOrderSql(int customerId)
        {
            return $@"

declare @customer_id int = ${customerId};
declare @order_id table (id uniqueidentifier);

insert into [dbo].[Orders] ([CustomerID], [EmployeeID], [OrderDate], [RequiredDate], [ShippedDate], [ShipVia], [Freight])
output inserted.[OrderId] into @order_id
values (@customer_id, 2, convert(date, sysutcdatetime()), convert(date, sysutcdatetime()), null, 0, 25);

insert into [dbo].[Order Details] ([CustomerID], [OrderID], [ProductID], [UnitPrice], [Quantity], [Discount])
values (@customer_id, (select id from @order_id), 61, 10.00, 1, 0),
       (@customer_id, (select id from @order_id), 10, 18.25, 1, 0);

";
        }
    }
}
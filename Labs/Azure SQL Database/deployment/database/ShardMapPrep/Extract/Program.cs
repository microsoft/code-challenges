using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Newtonsoft.Json;
using Prepare;

namespace Extract
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                Connects to the configured Order Shard database and extracts the Order and Order Details tables out to JSON files.
            */

            var connectionString = new SqlConnectionStringBuilder
            {
                DataSource = "<server>.database.windows.net",
                InitialCatalog = "<database>",
                UserID = "<username>",
                Password = "<password>"
            }
            .ToString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var orders = conn.Query<OrderRecord>(sql: "select CustomerID, OrderID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, Freight from dbo.[Orders]", commandTimeout: 0);
                WriteJsonToFile(orders, "orders.json");

                var details = conn.Query<OrderDetailRecord>(sql: "select CustomerID, OrderID, ProductID, UnitPrice, Quantity, Discount from dbo.[Order Details]", commandTimeout: 0);
                WriteJsonToFile(details, "order details.json");
            }
        }

        private static void WriteJsonToFile<T>(IEnumerable<T> records, string filename)
        {
            using (var file = File.OpenWrite(filename))
            using (var writer = new StreamWriter(file))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.Formatting = Formatting.Indented;

                new JsonSerializer().Serialize(jsonWriter, records);
            }
        }
    }
}

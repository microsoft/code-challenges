using System.Data.SqlClient;
using System.Web.Mvc;
using LabWeb.Helpers;

namespace LabWeb.Controllers
{
    public class Scenario2Controller : Controller
    {
        public ActionResult Index()
        {
            // Connect to the "Head" database and issue an elastic query
            var connectionString = Config.GetHeadDatabaseConnectionString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = @"

-- Query for all line items on all orders by a specific customer

select c.ContactName, o.OrderId, o.OrderDate, od.ProductID, p.ProductName, (od.Quantity * od.UnitPrice) as line_item_total
from [dbo].[Orders] o
left join [dbo].[Order Details] od on od.OrderId = o.OrderId
left join [dbo].[Products] p on p.ProductId = od.ProductId
left join [dbo].[Customers] c on c.CustomerId = o.CustomerId
where o.CustomerId = 25;

";

                    using (var reader = cmd.ExecuteReader())
                    {
                        var results = ResultsHelper.ReadResults(reader);

                        // Render the results!
                        return View(new ResultsViewModel(results, cmd.CommandText, reader.RecordsAffected));
                    }
                }
            }
        }
    }
}
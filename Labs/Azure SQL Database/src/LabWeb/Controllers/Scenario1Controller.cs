using System.Data.SqlClient;
using System.Web.Mvc;
using LabWeb.Helpers;

namespace LabWeb.Controllers
{
    public class Scenario1Controller : Controller
    {
        public ActionResult Index()
        {
            // Connect to the "Head" database and issue a query against the "Products" remote table
            var connectionString = Config.GetHeadDatabaseConnectionString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = @"

select 'external' as source, name from sys.external_tables;

select 'local' as source, name from sys.tables where schema_name(schema_id) = 'dbo';

select 'shard' as source, ServerName, DatabaseName from [__ShardManagement].[ShardsGlobal];

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

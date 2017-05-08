using System.Data.SqlClient;

namespace LabWeb
{
    internal static class Config
    {
        private static int ConnectTimeoutSeconds = 120;

        public const string ServerDomainName = "";
        public const string Username = "ENTER YOUR AZURE SQL DATABASE LOGICAL SERVER USERNAME";
        public const string Password = "ENTER YOUR AZURE SQL DATABASE LOGICAL SERVER PASSWORD";

        public const string HeadDatabase = "Head";

        /// <summary>
        /// Returns a connection string for connecting to the Head database.
        /// </summary>
        public static string GetHeadDatabaseConnectionString()
        {
            var builder = new SqlConnectionStringBuilder {

                ConnectTimeout = ConnectTimeoutSeconds,

                DataSource = ServerDomainName,
                InitialCatalog = HeadDatabase,

                UserID = Username,
                Password = Password

            };

            return builder.ToString();
        }
        
        /// <summary>
        /// Returns a connection string with credentials for connecting to the Order shards.
        /// </summary>
        public static string GetOrderShardConnectionString()
        {
            var builder = new SqlConnectionStringBuilder {

                ConnectTimeout = ConnectTimeoutSeconds,

                UserID = Username,
                Password = Password

            };

            return builder.ToString();
        }
    }
}
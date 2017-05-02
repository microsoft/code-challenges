#region Copyright ©2017, Click2Cloud Inc. - All Rights Reserved
/* ------------------------------------------------------------------- *
*                            Click2Cloud Inc.                          *
*                  Copyright ©2016 - All Rights reserved               *
*                                                                      *
* Apache 2.0 License                                                   *
* You may obtain a copy of the License at                              * 
* http://www.apache.org/licenses/LICENSE-2.0                           *
* Unless required by applicable law or agreed to in writing,           *
* software distributed under the License is distributed on an "AS IS"  *
* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express  *
* or implied. See the License for the specific language governing      *
* permissions and limitations under the License.                       *
*                                                                      *
* -------------------------------------------------------------------  */
#endregion Copyright ©2017, Click2Cloud Inc. - All Rights Reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_core_mssql_app.Classes
{
    public class ConnectionSetting
    {
        internal static string CONNECTION_STRING
        {
            get
            {
                string _connectionString = string.Format("Data Source={0},{1}; Initial Catalog={2}; User ID=sa; Password={3}", 
                    MSSQL_SERVICE_HOST, MSSQL_SERVICE_PORT, SQLDB_DATABASE, SA_PASSWORD);

                return _connectionString;
            }
        }

        private static string MSSQL_SERVICE_HOST
        {
            get
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MSSQL_SERVICE_HOST")))
                {
                    return Environment.GetEnvironmentVariable("MSSQL_SERVICE_HOST");
                }

                return string.Empty;
            }
        }

        private static string SA_PASSWORD
        {
            get
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SA_PASSWORD")))
                {
                    return Environment.GetEnvironmentVariable("SA_PASSWORD");
                }

                return string.Empty;
            }
        }

        internal static string SQLDB_DATABASE
        {
            get
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SQLDB_DATABASE")))
                {
                    return Environment.GetEnvironmentVariable("SQLDB_DATABASE");
                }

                return "MVCPersonDB";
            }
        }

        internal static string MSSQL_SERVICE_PORT
        {
            get
            {
                if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MSSQL_SERVICE_PORT")))
                {
                    return Environment.GetEnvironmentVariable("MSSQL_SERVICE_PORT");
                }

                return "1433";
            }
        }
    }
}

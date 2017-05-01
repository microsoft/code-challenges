using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace LabWeb.Helpers
{
    public static class ResultsHelper
    {
        public static IList<ResultSet> ReadResults(SqlDataReader reader)
        {
            var results = new List<ResultSet>();
            var id = 0;

            do
            {
                var fields = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();

                var rows = new List<ResultRow>();

                while (reader.Read())
                {
                    rows.Add(ReadResultRow(reader));
                }

                results.Add(new ResultSet(++id, fields, rows));

            } while (reader.NextResult());

            return results;
        }

        private static ResultRow ReadResultRow(SqlDataReader reader)
        {
            var values = Enumerable.Range(0, reader.FieldCount).Select(reader.GetValue).ToArray();

            return new ResultRow(values);
        }
    }
}
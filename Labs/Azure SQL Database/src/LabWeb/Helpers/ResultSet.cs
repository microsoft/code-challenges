using System.Collections.Generic;
using System.Linq;

namespace LabWeb.Helpers
{
    public class ResultSet
    {
        public ResultSet(int id, IEnumerable<string> names, IEnumerable<ResultRow> rows)
        {
            Id = id;
            Fields = (names ?? Enumerable.Empty<string>()).ToArray();
            Rows = (rows ?? Enumerable.Empty<ResultRow>()).ToArray();
        }

        public int Id { get; }

        public string[] Fields { get; }

        public ResultRow[] Rows { get; }
    }
}
using System.Collections.Generic;

namespace LabWeb.Helpers
{
    public class ResultRow
    {
        public ResultRow(object[] values)
        {
            Values = values;
        }

        public IEnumerable<object> Values { get; }
    }
}
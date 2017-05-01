using System.Collections.Generic;
using System.Linq;

namespace LabWeb.Helpers
{
    public class ResultsViewModel
    {
        public ResultsViewModel(IEnumerable<ResultSet> resultSets, string commandText, int recordsAffected)
        {
            CommandText = commandText;
            RecordsAffected = recordsAffected;
            ResultSets = (resultSets ?? Enumerable.Empty<ResultSet>()).ToArray();
        }

        public string CommandText { get; }

        public ResultSet[] ResultSets { get; }

        public int RecordsAffected { get; }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Search.Models;

namespace Microsoft.CodeChallenges.AzureSearch.Lab.SearchModels
{
    public class SearchResponse
    {
        public SearchResponse(DocumentSearchResult<JobResult> results = null)
        {
            if (results != null)
            {
                JobResults = results.Results.Select(x => x.Document).ToList();
                Facets = results.Facets;
            }
            else
            {
                JobResults = new List<JobResult>();
            }
        }
        public IList<JobResult> JobResults { get; set; }

        public FacetResults Facets { get; set; }
    }
}

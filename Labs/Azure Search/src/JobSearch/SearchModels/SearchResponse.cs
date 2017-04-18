using System.Collections.Generic;
using Microsoft.Azure.Search.Models;

namespace JobSearch.SearchModels
{
    public class SearchResponse
    {
        public SearchResponse()
        {
            JobResults = new List<JobResult>();
        }
        public IList<JobResult> JobResults { get; set; }

        public FacetResults Facets { get; set; }
    }
}

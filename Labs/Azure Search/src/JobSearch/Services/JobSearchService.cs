using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSearch.SearchModels;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace JobSearch.Services
{
    public class JobSearchService
    {
        private readonly string _apiKey;
        private readonly string _searchUrl;
        private readonly string _indexName;


        public static Dictionary<string, string> FacetDefinitions = new Dictionary<string, string>()
        {
            {"agency", "Agency"},
            {"posting_type", "Internal/External"},
            {"civil_service_title", "Common Job Title"}
        };

        public JobSearchService()
        {
            _searchUrl = "azsearch-build";
            _apiKey = "32D0CF6E6A03CDBD783DD3F6B390D735";
            _indexName = "jobs";
        }

        private SearchIndexClient GetClient()
        {
            return new SearchIndexClient(_searchUrl, _indexName, new SearchCredentials(_apiKey));
        }

        public async Task<List<string>> ExecuteSuggest(string query)
        {
            //TO DO - Place holder for suggest
            return new List<string>();
        }

        public async Task<DocumentSearchResult<JobResult>> ExecuteSearch(string query, List<FacetGroup> facets = null, PositionDistanceSearch geoSearch = null)
        {
            var searchParameters = new SearchParameters()
            {
                QueryType = QueryType.Full,
                SearchMode = SearchMode.All,
            };
            using (var indexClient = GetClient())
            {
                return await indexClient.Documents.SearchAsync<JobResult>(query, searchParameters);
            }
        }

        private string CreateFilter(List<FacetGroup> facets, PositionDistanceSearch geoSearch = null)
        {
            if (facets != null)
            {
                var query = new StringBuilder();
                var groupCount = facets.Count(e => e.FacetValues.Any(f => f.IsSelected ?? false));
                var groupCounter = 0;

                foreach (var facet in facets)
                {
                    var selectedValues = facet.FacetValues.Where(e => e.IsSelected ?? false).ToArray();
                    ;
                    if (selectedValues.Length > 0)
                    {
                        int counter = 0;
                        query.Append("(");
                        foreach (var facetSelection in selectedValues)
                        {
                            query.Append($"{facet.FacetName} eq '{facetSelection.FacetValue}'");
                            if (counter < selectedValues.Length - 1)
                            {
                                query.Append(" or ");
                            }
                            counter++;
                        }
                        query.Append(")");
                        if (groupCounter < groupCount - 1)
                        {
                            query.Append(" and ");
                        }
                        groupCounter++;
                    }
                }

                return query.ToString();
            }
            return null;
        }
    }
}

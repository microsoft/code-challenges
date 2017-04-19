using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobSearch.SearchModels;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace JobSearch.Services
{
    public class JobSearchService
    {
        private readonly string _apiKey;
        private readonly string _searchUrl;
        private readonly string _indexName;
        private readonly string _apiVersion;
        private readonly string _autocompleteMode;
        private readonly string _synonymMap;


        public static Dictionary<string, string> FacetDefinitions = new Dictionary<string, string>()
        {
            {"agency", "Agency"},
            {"posting_type", "Internal/External"},
            {"civil_service_title", "Common Job Title"}
        };

        public JobSearchService()
        {
            _searchUrl = "azsearch-build-bak";
            _apiKey = "FCAF9728436469CBF9A6F5F36A483EF0";
            _indexName = "jobsearch";
            _apiVersion = "2015-02-28-Preview";
            _autocompleteMode = "oneTerm"; //oneTerm, twoTerms, oneTermWithContext
            _synonymMap = "mysynonymmap";
        }

        private SearchIndexClient GetClient()
        {
            return new SearchIndexClient(_searchUrl, _indexName, new SearchCredentials(_apiKey));
        }

        private SearchServiceClient GetServiceClient()
        {
            return new SearchServiceClient(_searchUrl, new SearchCredentials(_apiKey));
        }
        public async Task<List<string>> ExecuteSuggest(string query)
        {
            using (var indexClient = GetClient())
            {
                // Query the Azure Search index for search suggestions
                var ap = new AutocompleteParameters()
                {
                    UseFuzzyMatching = true,
                    Top = 8
                };
                //var results = await indexClient.Documents.SuggestAsync(query, "sg", sp);
                //return results.Results.Select(e => e.Text).Distinct().ToList();

                var results = await SuggestAutocomplete(query, "sg", ap);
                // Extract the query plus autocomplete from the result set
                return results.Results.Select(e => e.QueryPlusText).Distinct().ToList();
            }

        }

        
        public async Task<SearchResponse> ExecuteSearch(string query, List<FacetGroup> facets = null, PositionDistanceSearch geoSearch = null)
        {
            // To remove as part of lab,
            var searchParameters = new SearchParameters()
            {
                QueryType = QueryType.Full,
                SearchMode = SearchMode.All,
                Facets = FacetDefinitions.Select(e => e.Key).ToList(), 
                Filter = CreateFilter(facets, geoSearch)
            };
            using (var indexClient = GetClient())
            {
                var queryResult = await indexClient.Documents.SearchAsync<JobResult>(query, searchParameters);
                return new SearchResponse
                {
                    JobResults = queryResult.Results.Select(x => x.Document).ToList(),
                    Facets = queryResult.Facets
                };
            }
            // To remove as part of lab,

            //return new SearchResponse();
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
                // To remove as part of lab
                if (geoSearch != null)
                {
                    if (query.Length > 0)
                    {
                        query.Append(" and ");
                    }
                    var lat = geoSearch.GeoPoint.Position.Latitude;
                    var lon = geoSearch.GeoPoint.Position.Longitude;
                    query.Append($"geo.distance(geo_location, geography'POINT({lon} {lat})') le {geoSearch.Radius}");
                }
                // To remove as part of lab
                return query.ToString();
            }

            return null;
        }

        /// <summary>
        /// As the current .NET client lib for azure search does not support autocomplete at this time we manually have to call the rest endpoint
        /// </summary>
        private async Task<AutocompleteResults> SuggestAutocomplete(string query, string suggestorName, AutocompleteParameters ap)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", _apiKey);
                var paramString = BuildParamString(query, suggestorName, ap);
                var url = $"https://{_searchUrl}.search.windows.net/indexes/{_indexName}/docs/autocomplete?{paramString}";
                using (var httpResponse = await client.GetAsync(url))
                {
                    var resultString = await httpResponse.Content.ReadAsStringAsync();
                    var resultObj = JsonConvert.DeserializeObject<AutocompleteResults>(resultString);
                    return resultObj;
                }
            }
        }

        /// <summary>
        /// We are manually building a parameter string to be used in our rest call.
        /// </summary>
        private string BuildParamString(string query, string suggestorName, AutocompleteParameters ap)
        {
            var sb = new StringBuilder();
            sb.Append($"api-version={_apiVersion}&search={query}&suggesterName={suggestorName}&autocompleteMode={_autocompleteMode}");

            if (ap.UseFuzzyMatching != null)
            {
                sb.Append($"&fuzzy={ap.UseFuzzyMatching.ToString().ToLower()}");
            }
            if (ap.Top != null)
            {
                sb.Append($"&$top={ap.Top}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// As the current .NET client lib for azure search does not support autocomplete at this time we manually have to call the rest endpoint
        /// </summary>
        public async Task<List<string>> GetSynonymMap()
        {
            using (var serviceClient = GetServiceClient())
            {
                var response = await serviceClient.SynonymMaps.GetAsync(_synonymMap);
                return response.Synonyms.Split(',').Select(x => x.Trim()).ToList();
            }
        }

        private class AutocompleteResults
        {
            [JsonProperty("value")]
            public AutocompleteResult[] Results { get; set; }
        }
        private class AutocompleteResult
        {
            public string Text { get; set; }
            public string QueryPlusText { get; set; }
        }

        private class AutocompleteParameters
        {
            public bool? UseFuzzyMatching { get; set; }
            public int? Top { get; set; }

        }
    }
}

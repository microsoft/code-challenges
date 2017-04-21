using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using System.Net.Http;
using Microsoft.Azure.Search.Models;
using Microsoft.CodeChallenges.AzureSearch.Lab.Configuration;
using Microsoft.CodeChallenges.AzureSearch.Lab.SearchModels;
using Newtonsoft.Json;

namespace Microsoft.CodeChallenges.AzureSearch.Lab.Services
{
    public class JobSearchService
    {
        private readonly string _apiKey;
        private readonly string _searchUrl;
        private const string IndexName = "jobsearch";
        private const string ApiVersion = "2015-02-28-Preview";
        private const string AutocompleteMode = "oneTerm"; //oneTerm, twoTerms, oneTermWithContext
        private const string SynonymMap = "mysynonymmap";
        
        public static Dictionary<string, string> FacetDefinitions = new Dictionary<string, string>()
        {
            {"agency", "Agency"},
            {"posting_type", "Internal/External"},
            {"civil_service_title", "Common Job Title"}
        };

        public JobSearchService()
        {
            _searchUrl = SearchConfig.SearchName;
            _apiKey = SearchConfig.SearchApiKey;
        }

        private SearchIndexClient GetClient()
        {
            return new SearchIndexClient(_searchUrl, IndexName, new SearchCredentials(_apiKey));
        }

        private SearchServiceClient GetServiceClient()
        {
            return new SearchServiceClient(_searchUrl, new SearchCredentials(_apiKey));
        }
        public async Task<List<string>> ExecuteSuggest(string query)
        {
            //TO DO - Place holder for suggest
            return new List<string>();
        }

        
        public async Task<SearchResponse> ExecuteSearch(string query, List<FacetGroup> facets = null, PositionDistanceSearch geoSearch = null)
        {
            //TO DO - Place holder for search
            return new SearchResponse();
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

                return query.ToString();
            }

            return null;
        }

        /// <summary>
        /// As the current .NET client lib for azure search does not support autocomplete at this time we manually have to call the rest endpoint
        /// </summary>
        private async Task<AutocompleteResults> SuggestAutocompleteAsync(string query, string suggestorName, AutocompleteParameters ap)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", _apiKey);
                var paramString = BuildParamString(query, suggestorName, ap);
                var url = $"https://{_searchUrl}.search.windows.net/indexes/{IndexName}/docs/autocomplete?{paramString}";
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
            sb.Append($"api-version={ApiVersion}&search={query}&suggesterName={suggestorName}&autocompleteMode={AutocompleteMode}");

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
                var response = await serviceClient.SynonymMaps.GetAsync(SynonymMap);
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

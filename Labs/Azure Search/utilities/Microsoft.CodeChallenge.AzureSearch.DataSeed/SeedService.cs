using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;

namespace Microsoft.CodeChallenge.AzureSearch.DataSeed
{
    public class SeedService
    {
        private const string IndexName = "jobsearch";
        private const string SynonymMapName = "mysynonymmap";
        private const string SynonymMap = @"engineer, designer, planner, builder, architect, producer, fabricator, developer, creator";
        private const string ApiVersion = "2015-02-28-Preview";

        private readonly string _searchInstanceName;
        private readonly string _apiKey;

        public SeedService(string searchInstanceName, string apiKey)
        {
            _searchInstanceName = searchInstanceName;
            _apiKey = apiKey;
        }
        
        private SearchIndexClient GetIndexClient(string index)
        {
            return new SearchIndexClient(_searchInstanceName, index, new SearchCredentials(_apiKey));
        }

        private SearchServiceClient GetServiceClient()
        {
            return new SearchServiceClient(_searchInstanceName, new SearchCredentials(_apiKey));
        }

        public async Task ExecuteAsync()
        {
            await LoadSynonymMap();
            await LoadIndexAsync();
            await LoadDataSetAsync();
        }

        private async Task LoadIndexAsync()
        {
            if (!await IndexExistsAsync())
            {
                await CreateIndexAsync();
            }
        }

        private async Task<bool> IndexExistsAsync()
        {
            using (var client = GetServiceClient())
            {
                var result = await client.Indexes.ExistsAsync(IndexName);
                Console.WriteLine(result ? "Search index exists" : "Search index does not exist");
                return result;
            }
        }

        private async Task LoadSynonymMap()
        {
            using (var serviceClient = GetServiceClient())
            {
                var synonymMap = new SynonymMap(SynonymMapName, SynonymMapFormat.Solr, SynonymMap);
                await serviceClient.SynonymMaps.CreateOrUpdateAsync(synonymMap);
                Console.WriteLine("Created/Updated Synonym Map");
            }
        }

        /// <summary>
        /// Creating index using a json file as the .net framework does not have everything we need
        /// </summary>
        private async Task CreateIndexAsync()
        {
            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(JobSearchIndex);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.Add("api-key", _apiKey);
                using (var response = await httpClient.PostAsync(
                    $"https://{_searchInstanceName}.search.windows.net/indexes?api-version={ApiVersion}", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Created search index");
                    }
                    else
                    {
                        var message = await response.Content.ReadAsStringAsync();
                        var errorMessage = $"There was a error trying to create the index; Error Message {message}";
                        Console.WriteLine(errorMessage);
                        Console.ReadLine();

                        throw new HttpRequestException(errorMessage);
                    }
                }
            }
        }

        private async Task LoadDataSetAsync()
        {
            using (var client = GetIndexClient(IndexName))
            {
                var dataFolder = System.IO.Directory.GetFiles(".\\data");
                var documentCount = dataFolder.Length;
                Console.WriteLine($"Uploading {documentCount} documents ");
                foreach (var file in dataFolder)
                {
                    await LoadDataFileAsync(file, client);
                    Console.Write(".");
                }
                Console.WriteLine($"Uploaded {documentCount} documents ");
            }
        }

        private async Task LoadDataFileAsync(string file, SearchIndexClient indexClient)
        {
            using (var sr = System.IO.File.OpenText(file))
            {
                var json = sr.ReadToEnd();
                var documents = JsonConvert.DeserializeObject<List<dynamic>>(json);
                var actions = new List<IndexAction<dynamic>>();
                foreach (var doc in documents)
                {
                    var x = IndexAction.MergeOrUpload<dynamic>(doc);
                    actions.Add(x);
                }
                var batch = new IndexBatch<dynamic>(actions);
                await indexClient.Documents.IndexAsync(batch);
            }
        }

        private const string JobSearchIndex = @"{'name':'jobsearch','fields':[{'name':'id','type':'Edm.String','searchable':false,'filterable':false,'retrievable':true,'sortable':false,'facetable':false,'key':true,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'job_id','type':'Edm.String','searchable':false,'filterable':false,'retrievable':true,'sortable':false,'facetable':false,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'agency','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.lucene','synonymMaps':[]},{'name':'agency_phonetic','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'my_standard','synonymMaps':[]},{'name':'posting_type','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'num_of_positions','type':'Edm.Int32','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'business_title','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.lucene','synonymMaps':['mysynonymmap']},{'name':'business_title_phonetic','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'my_standard','synonymMaps':[]},{'name':'civil_service_title','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.lucene','synonymMaps':[]},{'name':'civil_service_title_phonetic','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'my_standard','synonymMaps':[]},{'name':'title_code_no','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'level','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'salary_range_from','type':'Edm.Int32','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'salary_range_to','type':'Edm.Int32','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'salary_frequency','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'work_location','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'division_work_unit','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.lucene','synonymMaps':[]},{'name':'division_work_unit_phonetic','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'my_standard','synonymMaps':[]},{'name':'job_description','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':false,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.microsoft','synonymMaps':[]},{'name':'minimum_qual_requirements','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.microsoft','synonymMaps':[]},{'name':'preferred_skills','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.microsoft','synonymMaps':[]},{'name':'additional_information','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':'en.microsoft','synonymMaps':[]},{'name':'to_apply','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'hours_per_shift','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'recruitment_contact','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'residency_requirement','type':'Edm.String','searchable':true,'filterable':true,'retrievable':true,'sortable':true,'facetable':false,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'posting_date','type':'Edm.DateTimeOffset','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'post_until','type':'Edm.DateTimeOffset','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'posting_updated','type':'Edm.DateTimeOffset','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'process_date','type':'Edm.DateTimeOffset','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'geo_location','type':'Edm.GeographyPoint','searchable':false,'filterable':true,'retrievable':true,'sortable':true,'facetable':false,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]},{'name':'tags','type':'Collection(Edm.String)','searchable':true,'filterable':true,'retrievable':true,'sortable':false,'facetable':true,'key':false,'indexAnalyzer':null,'searchAnalyzer':null,'analyzer':null,'synonymMaps':[]}],'scoringProfiles':[{'name':'jobsScoringFeatured','text':{'weights':{'business_title':3,'civil_service_title':3}},'functions':[{'fieldName':'posting_date','freshness':{'boostingDuration':'P500D'},'interpolation':'linear','magnitude':null,'distance':null,'tag':null,'type':'freshness','boost':3},{'fieldName':'tags','freshness':null,'interpolation':'linear','magnitude':null,'distance':null,'tag':{'tagsParameter':'featuredParam'},'type':'tag','boost':10},{'fieldName':'geo_location','freshness':null,'interpolation':'linear','magnitude':null,'distance':{'referencePointParameter':'mapCenterParam','boostingDistance':5},'tag':null,'type':'distance','boost':6}],'functionAggregation':'sum'}],'defaultScoringProfile':null,'corsOptions':null,'suggesters':[{'name':'sg','searchMode':'analyzingInfixMatching','sourceFields':['posting_type','business_title','agency','civil_service_title','work_location','division_work_unit']}],'analyzers':[{'@odata.type':'#Microsoft.Azure.Search.CustomAnalyzer','name':'my_standard','tokenizer':'standard','tokenFilters':['lowercase','asciifolding','phonetic'],'charFilters':[]}],'tokenizers':[],'tokenFilters':[],'charFilters':[]}";
    }
}
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace Microsoft.CodeChallenges.CosmosDB.Lab.Controllers
{
    public class InsertController : Controller
    {
        private DocumentClient _client;
        private Uri _collectionUri;

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Seed()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            var file = System.IO.File.ReadAllText(Path.Combine(filePath, @"Seed/Tweets.json"));
            var documents = JsonConvert.DeserializeObject<dynamic[]>(file);
            var (client, uri) = await GetDocumentClient();
            foreach (var document in documents)
            {
                await client.UpsertDocumentAsync(uri, document);
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Index(FormCollection collection)
        {
            var insertData = collection["insertData"];
            if (string.IsNullOrEmpty(insertData))
                ModelState.AddModelError("", "No input for JSON");

            try
            {
                object document = JsonConvert.DeserializeObject(insertData);
                if (document != null)
                    try
                    {
                        var (client, uri) = await GetDocumentClient();
                        var documentResponse = await client.UpsertDocumentAsync(uri, document);
                        ViewBag.SavedDocument = JsonConvert.SerializeObject(documentResponse.Resource, Formatting.Indented);
                        ViewBag.Success = "JSON successfully Saved to Cosmos DB!";
                    }
                    catch
                    {
                        ModelState.AddModelError("", "Failed to create document in Cosmos DB");
                        ViewBag.InsertData = insertData;
                    }
            }
            catch
            {
                ModelState.AddModelError("", "Input is not valid JSON");
                ViewBag.InsertData = insertData;
            }
            return View();
        }

        private async Task<(DocumentClient, Uri)> GetDocumentClient()
        {
            if (_client == null)
            {
                var dbEndpoint = new Uri(ConfigurationManager.AppSettings["CosmosDB:Endpoint"]);
                var dbKey = ConfigurationManager.AppSettings["CosmosDB:Key"];
                var databaseName = ConfigurationManager.AppSettings["CosmosDB:DatabaseName"];
                var collectionName = ConfigurationManager.AppSettings["CosmosDB:CollectionName"];
                _client = new DocumentClient(dbEndpoint, dbKey);
                _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

                await _client.CreateDatabaseIfNotExistsAsync(new Database {Id = databaseName});
                await _client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    new DocumentCollection {Id = collectionName});

                try
                {
                    await _client.CreateUserDefinedFunctionAsync(_collectionUri, new UserDefinedFunction()
                    {
                        Id = "displayDate",
                        Body = @"function displayDate(inputDate) {
                                return inputDate.split('T')[0];
                            }"
                    });
                }
                catch (DocumentClientException)
                {
                    //Ignore - It probably already exists
                }
            }

            return (_client, _collectionUri);
        }
    }
}
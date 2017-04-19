using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using LabWeb.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace LabWeb.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string[] _availableRegions = { LocationNames.WestUS, LocationNames.CentralUS, LocationNames.NorthEurope, LocationNames.SoutheastAsia };
        
        private static Dictionary<string, DocumentClient> _readonlyClients;
        
        public async Task<ActionResult> Index()
        {
            // this is initializing the client connections.
            await GetReadOnlyClient();
            return View(new HomeViewModel(_availableRegions));
        }

        private async Task<DocumentClient> GetReadOnlyClient(string locationName = null)
        {
            if (_readonlyClients == null)
            {
                var dbEndpoint = new Uri(ConfigurationManager.AppSettings["DocumentDBEndpoint"]);
                var dbKey = ConfigurationManager.AppSettings["DocumentDBPrimaryReadonlyKey"];

                var clients = new Dictionary<string, DocumentClient>();
                var tasks = new List<Task>();
                foreach (var region in _availableRegions)
                {
                    var policy = new ConnectionPolicy()
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp,
                        PreferredLocations = { region }
                    };
                    var client = new DocumentClient(dbEndpoint, dbKey, policy);
                    tasks.Add(Task.Run(async () =>
                    {
                         await client.OpenAsync();
                         await WarmupConnection(client);
                     }));
                    
                    clients.Add(region, client);
                }
                _readonlyClients = clients;
                await Task.WhenAll(tasks);
            }

            if (locationName == null)
            {
                return null;
            }
            return _readonlyClients[locationName];
        }

        private const string WarmupQueryOne = "SELECT * FROM c";
        private const string WarmupQueryTwo = "SELECT COUNT(1) FROM c";
        private static readonly FeedOptions _feedOptions = new FeedOptions { MaxItemCount = 10, EnableScanInQuery = true };
        private async Task WarmupConnection(IDocumentClient client)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(ConfigurationManager.AppSettings["DocumentDBName"], ConfigurationManager.AppSettings["DocumentDBCollectionName"]);
            var query = client.CreateDocumentQuery(collectionUri, WarmupQueryOne, _feedOptions).AsDocumentQuery();
            await query.ExecuteNextAsync();
            query = client.CreateDocumentQuery(collectionUri, WarmupQueryTwo, _feedOptions).AsDocumentQuery();
            await query.ExecuteNextAsync();
        }

        public async Task<ActionResult> Query(string query, string locationName)
        {
            var newModel = new QueryModel
            {
                Documents = new List<string>(),
                Query = query
            };
            int numRetries = 0;
            var collectionUri = UriFactory.CreateDocumentCollectionUri(ConfigurationManager.AppSettings["DocumentDBName"], ConfigurationManager.AppSettings["DocumentDBCollectionName"]);
            var client = await GetReadOnlyClient(locationName);
            IDocumentQuery<dynamic> docQuery = client.CreateDocumentQuery(collectionUri, query, _feedOptions).AsDocumentQuery();

            if (docQuery != null)
            {
                do
                {
                    try
                    {
                        var sw = Stopwatch.StartNew();
                        var results = await docQuery.ExecuteNextAsync();
                        sw.Stop();
                        newModel.ResponseTime = sw.ElapsedMilliseconds;

                        foreach (dynamic result in results)
                        {
                            string json = result.ToString();
                            string formattedJson = (json.StartsWith("{", StringComparison.InvariantCulture) ||
                                                    json.StartsWith("[", StringComparison.InvariantCulture))
                                ? JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented)
                                : json;
                            newModel.Documents.Add(formattedJson);
                            newModel.Count++;
                        }
                        newModel.Error = null;
                        newModel.StatusCode = 200;
                        break;
                    }
                    catch (Exception e)
                    {
                        newModel.Documents.Clear();
                        numRetries++;
                        var exception = e.InnerException as DocumentClientException;
                        if (exception != null)
                        {
                            if (exception.StatusCode != null && (int)exception.StatusCode == 429)
                            {
                                numRetries--;
                            }
                            int startIndex = exception.Message.IndexOf("{", StringComparison.InvariantCulture);
                            int endIndex = exception.Message.LastIndexOf("}", StringComparison.InvariantCulture) + 1;
                            newModel.Error = (startIndex < 0 || endIndex < 0)
                                ? exception.Message
                                : exception.Message.Substring(startIndex, endIndex - startIndex);
                            if (exception.StatusCode != null) newModel.StatusCode = (int)exception.StatusCode;
                        }
                        else
                        {
                            newModel.Error = e.Message.Substring(0, e.Message.IndexOf(Environment.NewLine, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                } while (numRetries < 1);
            }

            Response.Write(JsonConvert.SerializeObject(newModel));
            return null;
        }
    }
}
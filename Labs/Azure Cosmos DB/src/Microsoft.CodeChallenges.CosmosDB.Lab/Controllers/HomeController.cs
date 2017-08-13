using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.CodeChallenges.CosmosDB.Lab.Models;
using Newtonsoft.Json;

namespace Microsoft.CodeChallenges.CosmosDB.Lab.Controllers
{
    public class HomeController : Controller
    {
        private static readonly string[] _availableRegions = {LocationNames.WestUS, LocationNames.CentralUS, LocationNames.NorthEurope, LocationNames.SoutheastAsia};

        private static Dictionary<string, DocumentClient> _readonlyClients;

        private static readonly FeedOptions FeedOptions = new FeedOptions {MaxItemCount = 10, EnableScanInQuery = true};

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
                var dbEndpoint = new Uri(ConfigurationManager.AppSettings["CosmosDB:Endpoint"]);
                var dbKey = ConfigurationManager.AppSettings["CosmosDB:Key"];

                var clients = new Dictionary<string, DocumentClient>();
                var tasks = new List<Task>();
                foreach (var region in _availableRegions)
                {
                    var policy = new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp,
                        PreferredLocations = {region}
                    };
                    var client = new DocumentClient(dbEndpoint, dbKey, policy);
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

        public async Task<ActionResult> Query(string query, string locationName)
        {
            var newModel = new QueryModel
            {
                Documents = new List<string>(),
                Query = query
            };
            var numRetries = 0;

            IDocumentQuery<dynamic> docQuery = null;

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

                        foreach (var result in results)
                        {
                            string json = result.ToString();
                            var formattedJson = json.StartsWith("{", StringComparison.InvariantCulture) ||
                                                json.StartsWith("[", StringComparison.InvariantCulture)
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
                            if (exception.StatusCode != null && (int) exception.StatusCode == 429)
                            {
                                numRetries--;
                            }
                            var startIndex = exception.Message.IndexOf("{", StringComparison.InvariantCulture);
                            var endIndex = exception.Message.LastIndexOf("}", StringComparison.InvariantCulture) + 1;
                            newModel.Error = startIndex < 0 || endIndex < 0
                                ? exception.Message
                                : exception.Message.Substring(startIndex, endIndex - startIndex);
                            if (exception.StatusCode != null)
                            {
                                newModel.StatusCode = (int) exception.StatusCode;
                            }
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
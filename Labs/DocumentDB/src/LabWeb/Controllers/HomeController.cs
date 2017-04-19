using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
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

        private object obj = new object();
        private Dictionary<string, DocumentClient> _readonlyClients;

        public ActionResult Index()
        {
            return View(new HomeViewModel(_availableRegions));
        }
        
        public DocumentClient GetReadOnlyClient(string locationName)
        {
            if (_readonlyClients == null)
            {
                lock (obj)
                {
                    var dbEndpoint = new Uri(ConfigurationManager.AppSettings["DocumentDBEndpoint"]);
                    var dbKey = ConfigurationManager.AppSettings["DocumentDBPrimaryReadonlyKey"];

                    _readonlyClients = _availableRegions.ToDictionary(location => location,
                                location => new DocumentClient(dbEndpoint, dbKey, new ConnectionPolicy() { ConnectionMode = ConnectionMode.Direct, PreferredLocations = { location } }));
                }
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
            int numRetries = 0;
            var collectionUri = UriFactory.CreateDocumentCollectionUri(ConfigurationManager.AppSettings["DocumentDBName"], ConfigurationManager.AppSettings["DocumentDBCollectionName"]);
            IDocumentQuery<dynamic> docQuery = GetReadOnlyClient(LocationNames.SoutheastAsia).CreateDocumentQuery(
                collectionUri,
                query,
                new FeedOptions
                {
                    MaxItemCount = 10,
                    EnableScanInQuery = true
                }
            ).AsDocumentQuery();

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
                            newModel.Error = e.Message;
                        }
                    }
                } while (numRetries < 1);
            }

            Response.Write(JsonConvert.SerializeObject(newModel));
            return null;
        }
    }
}
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;

namespace DocumentDBSeeder
{
    static class Program
    {
        private static DocumentClient _client;

        static void Main(string[] args)
        {
            var endpoint = ConfigurationManager.AppSettings["endpoint"];
            var masterKey = ConfigurationManager.AppSettings["key"];
            var db = ConfigurationManager.AppSettings["database"];
            var coll = ConfigurationManager.AppSettings["collection"];


            try
            {
                using (_client = new DocumentClient(new Uri(endpoint), masterKey, new ConnectionPolicy()))
                {
                    RunUpdateAsync(coll, db).Wait();
                }
            }
            catch (Exception e)
            {
                // If the Exception is a DocumentClientException, the "StatusCode" value might help identity 
                // the source of the problem. 
                Console.WriteLine("Perf Counter runner failed with exception:{0}", e);
            }
            finally
            {
                Console.WriteLine("Perf Counter runner emission ends.");
                Console.ReadKey();
            }

        }
        private static async Task RunUpdateAsync(string collection, string database)
        {
            Uri collUri = UriFactory.CreateDocumentCollectionUri(database, collection);

            try
            {
                await _client.ReadDocumentCollectionAsync(collUri);
            }
            catch (DocumentClientException ex)
            {
                //Simply return false if this is the error. Else throw it.
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    await
                   _client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(database),
                       new DocumentCollection() { Id = collection });
                }
            }

            var categoryCounters = new[]
            {
                new PerformanceCounterCategory("LogicalDisk"),
                new PerformanceCounterCategory("PhysicalDisk"),
                new PerformanceCounterCategory("HTTP Service"),
                new PerformanceCounterCategory("ASP.NET v4.0.30319"),
                new PerformanceCounterCategory("Power Meter"),
                new PerformanceCounterCategory("Processor"),
                new PerformanceCounterCategory("Energy Meter"),
                new PerformanceCounterCategory("Node.js"),
            };

            while (true)
            {
                DateTime dt = DateTime.UtcNow;
                TimeSpan sleepTime = TimeSpan.FromSeconds(5);
                try
                {
                    var counterMetric = new JObject
                            {
                                {"id", Guid.NewGuid()},
                                {"timestamp", dt},
                                {"machineName", Environment.MachineName},
                            };

                    var counterArray = new JArray();

                    foreach (var category in categoryCounters)
                    {
                        foreach (var instance in category.GetInstanceNames())
                        {
                            var instanceObject = new JObject()
                            {
                                {"counterType", category.CategoryName},
                                {"counterFor", instance}
                            };

                            foreach (var counter in category.GetCounters(instance))
                            {
                                instanceObject.Add(counter.CounterName, counter.RawValue);
                            }

                            counterArray.Add(instanceObject);
                        }
                    }

                    counterMetric.Add("logs", counterArray);

                    await _client.CreateDocumentAsync(collUri, counterMetric);
                    Console.WriteLine(counterMetric);
                }
                catch (DocumentClientException de)
                {
                    if (de.StatusCode != null && (int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        throw;
                    }

                    DocumentClientException de = (DocumentClientException)ae.InnerException;
                    if (de.StatusCode != null && (int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }

                await Task.Delay(sleepTime);


            }


        }
    }
}

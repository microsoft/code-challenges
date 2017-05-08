using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Microsoft.CodeChallenges.CosmosDB.TweetSeed
{
    public class CosmosDbService
    {
        private DocumentClient _client;
        private Uri _collectionUri;

        public async Task Initalise()
        {
            var endpoint = CloudConfigurationManager.GetSetting("CosmosDB:Endpoint");
            var key = CloudConfigurationManager.GetSetting("CosmosDB:Key");

            _client = new DocumentClient(new Uri(endpoint), key);

            var database = CloudConfigurationManager.GetSetting("CosmosDB:DatabaseName");
            var collection = CloudConfigurationManager.GetSetting("CosmosDB:CollectionName");
            _collectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);

            try
            {
                await _client.CreateDatabaseAsync(new Database {Id = database});
            }
            catch (DocumentClientException)
            {
                //Ignore - It probably already exists
            }

            try
            {
                await _client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(database), new DocumentCollection { Id = collection });
            }
            catch (DocumentClientException)
            {
                //Ignore - It probably already exists
            }

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

            await _client.ReadDocumentCollectionAsync(_collectionUri);
        }

        public async Task UploadDocument(object document)
        {
            await _client.CreateDocumentAsync(_collectionUri, document);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace TweetTrackerWebJob
{
    public class DocumentDbService
    {
        private DocumentClient _client;
        private Uri _collectionUri;

        public async Task Initalise()
        {
            var endpoint = CloudConfigurationManager.GetSetting("endpoint");
            var key = CloudConfigurationManager.GetSetting("key");

            _client = new DocumentClient(new Uri(endpoint), key);

            var database = CloudConfigurationManager.GetSetting("database");
            var collection = CloudConfigurationManager.GetSetting("collection");
            _collectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);

            try
            {
                await _client.CreateDatabaseAsync(new Database() { Id = database });
                await _client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(database), new DocumentCollection() { Id = collection });
            }
            catch (DocumentClientException ex)
            {
               //Err?
            }

            await _client.ReadDocumentCollectionAsync(_collectionUri);
        }

        public async Task UploadDocument(object document)
        {
            await _client.CreateDocumentAsync(_collectionUri, document);
        }
    }
}

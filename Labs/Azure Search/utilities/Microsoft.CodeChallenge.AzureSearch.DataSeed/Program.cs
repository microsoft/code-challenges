using System;

namespace Microsoft.CodeChallenge.AzureSearch.DataSeed
{
    class Program
    {
        private const string SearchName = "<Azure Search Name Here>";
        private const string ApiKey = "<Azure Search Api Key Here>";

        static void Main(string[] args)
        {
            var seedService = new SeedService(SearchName, ApiKey);
            seedService.ExecuteAsync().Wait();
        }
    }
}

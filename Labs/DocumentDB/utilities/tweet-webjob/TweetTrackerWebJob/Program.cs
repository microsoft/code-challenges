using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace TweetTrackerWebJob
{
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();
            host.CallAsync(typeof(Functions).GetMethod(nameof(Functions.ListenForTweets)));
            host.RunAndBlock();
        }
    }
}

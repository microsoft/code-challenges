using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Tweetinvi.Models;
using Stream = Tweetinvi.Stream;

namespace TweetTrackerWebJob
{
    public class Functions
    {
        [NoAutomaticTrigger]
        public static async Task ListenForTweets(TextWriter log)
        {
            var credentials = new TwitterCredentials("2Zv90CCvlvDIyksuIYQj5prNG", "61AlZAT6vTmh4r6rR3cIXGR4LrXoVj0ptCaPwY6f2xOdDm6fT1", "349776177-gjqeP3L5jiavLOWr70YagnoQdJeWZFWvyXefqfK8", "TQACcmeZNzxgmACtiqI5p4GbWzVMthJDGk1vRKbPCV8fG");
            var stream = Stream.CreateFilteredStream(credentials);

            var docDbService = new DocumentDbService();
            await docDbService.Initalise();

            stream.AddTrack("#Trump", async (tweet) =>
            {
                await docDbService.UploadDocument(tweet);
                Console.WriteLine(tweet.FullText);
            });

            stream.StartStreamMatchingAllConditions();
        }
    }
}

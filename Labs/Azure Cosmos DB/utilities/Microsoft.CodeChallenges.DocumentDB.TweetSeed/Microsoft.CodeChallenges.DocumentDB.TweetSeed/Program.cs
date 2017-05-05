using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Tweetinvi;
using Tweetinvi.Models;

namespace Microsoft.CodeChallenges.DocumentDB.TweetSeed
{
    class Program
    {
        private static DocumentDbService _docDbService;

        private static void Main()
        {
            Task.WaitAll(Task.Run(async () =>
            {
                var credentials = new TwitterCredentials(CloudConfigurationManager.GetSetting("Twitter:ConsumerKey"),
                                                         CloudConfigurationManager.GetSetting("Twitter:ConsumerSecret"),
                                                         CloudConfigurationManager.GetSetting("Twitter:AccessToken"),
                                                         CloudConfigurationManager.GetSetting("Twitter:AccessTokenSecret"));

                _docDbService = new DocumentDbService();
                await _docDbService.Initalise();

                var tags = CloudConfigurationManager.GetSetting("Twitter:Tags").Split(',');

                TwitterLiveStream(credentials, tags);
                //TwitterSearch(credentials, tags);
            }));
        }

        private static void TwitterSearch(TwitterCredentials credentials, string[] tags)
        {
            Auth.SetCredentials(credentials);

            foreach (var tag in tags)
            {
                foreach (var tweet in Search.SearchTweets(tag))
                {
                    _docDbService.UploadDocument(tweet).ConfigureAwait(false);
                    Console.WriteLine(tweet.FullText);
                }
            }
        }

        private static void TwitterLiveStream(TwitterCredentials credentials, string[] tags)
        {
            var stream = Stream.CreateFilteredStream(credentials);

            foreach (var tag in tags)
            {
                stream.AddTrack(tag, async (tweet) =>
                {
                    try
                    {
                        await _docDbService.UploadDocument(tweet);
                        Console.WriteLine(tweet.FullText);
                    }
                    catch
                    {
                        //Ignore
                    }
                });
            }

            stream.StartStreamMatchingAllConditions();
        }
    }
}

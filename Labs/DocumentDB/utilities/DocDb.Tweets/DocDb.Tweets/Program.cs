using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Tweetinvi;
using Tweetinvi.Models;

namespace DocDb.Tweets
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

                var stream = Stream.CreateFilteredStream(credentials);

                foreach (var tag in CloudConfigurationManager.GetSetting("Twitter:Tags").Split(','))
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

            }));
        }
    }
}

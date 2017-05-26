using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TwitterHandle
{
    public class Twitter
    {
        private static readonly SingleUserAuthorizer Authorizer = new SingleUserAuthorizer
        {
            CredentialStore = new SingleUserInMemoryCredentialStore
            {
                ConsumerKey = "OqA5KyyHlzgp2uubEzbCNn7iA",
                ConsumerSecret = "kfijHLVSdAyZ67z7anwyZkPQYxwMiIjCvKBzsiO6kSXKRZaFdq",
                AccessToken = "1908011538-rKQk1k4QUpz3AKIqacWaspjO3ZP8BP7fyhtGeZY",
                AccessTokenSecret = "Mn8ORoqrrGaUwJEaOygEebImAfVVkdkU3MbhnjGAvDgHY"

                
            }
        };

        public static List<Status> SearchUserTweet(string screenName)
        {
            
                var twitterCtx = new TwitterContext(Authorizer);
                List<Status> searchResults = new List<Status>();
                int maxNumberToFind = 200;
                int pagination = 0;
                ulong lastId = 0;

                var u = GetUser(screenName);
            
                if (u == null) return null;
                int maxPagination = u.StatusesCount / 200;

                var tweets = (from tweet in twitterCtx.Status
                              where tweet.Type == StatusType.User &&
                                  tweet.ScreenName == screenName &&
                                  tweet.Count == maxNumberToFind
                              select tweet).ToList();

                if (tweets.Count > 0)
                {
                    lastId = ulong.Parse(tweets.Last().StatusID.ToString());
                    searchResults.AddRange(tweets);
                }

                do
                {
                    var id = lastId - 1;
                    tweets = (from tweet in twitterCtx.Status
                              where tweet.Type == StatusType.User &&
                                  tweet.ScreenName == screenName &&
                                  tweet.Count == maxNumberToFind &&
                                  tweet.MaxID == id
                              select tweet).ToList();

                    searchResults.AddRange(tweets);
                    lastId = tweets.Min(x => x.StatusID);
                    pagination++;
                    if (!(pagination < maxPagination) || pagination >= 16)
                    {
                        break;
                    }
                } while (true);
                return searchResults;
            
        }

        public static IOrderedEnumerable<Status> GetMostLikedTweets(string screenName)
        {

            var tweets = SearchUserTweet(screenName).OrderByDescending(x => x.FavoriteCount);
            return tweets;
        }

        public static IOrderedEnumerable<Status> GetMostReTweeted(string screenName)
        {
            var tweets = SearchUserTweet(screenName).OrderByDescending(x => x.RetweetCount);
            return tweets;
        }

        public static int GetTweetCount(string screenName)
        {
            var u = GetUser(screenName);
            return u?.StatusesCount ?? 0;
        }

        public static int GetFollowerCount(string screenName)
        {
            var u = GetUser(screenName);
            return u?.FollowersCount ?? 0;
        }

        public static int GetFollowingCount(string screenName)
        {
            var u = GetUser(screenName);
            return u?.FriendsCount ?? 0;
        }

        public static string GetAvatar(string screenName)
        {
            var u = GetUser(screenName);
            return u?.ProfileImageUrl;
        }

        public static User GetUser(string screenName)
        {
            var twitterCtx = new TwitterContext(Authorizer);

            try
            {
                var u = (from user in twitterCtx.User
                         where user.Type == UserType.Show && user.ScreenName == screenName
                         select user).SingleOrDefault();
                return u;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

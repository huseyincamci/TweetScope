using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using TweetScope.Web.UI.ViewModel;
using TwitterHandle;

namespace TweetScope.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Analyze(string screenName)
        {
            if (!string.IsNullOrEmpty(screenName))
            {
                if (Twitter.GetUser(screenName) != null)
                {
                    if (Twitter.GetUser(screenName).Protected != true)
                    {
                        screenName = screenName.ToLower();
                        ViewBag.ScreenName = screenName;
                        ViewBag.ImageUrl = Twitter.GetAvatar(screenName);
                        ViewBag.TweetCount = Twitter.GetTweetCount(screenName);
                        ViewBag.FollowersCount = Twitter.GetFollowerCount(screenName);
                        ViewBag.FollowingCount = Twitter.GetFollowingCount(screenName);
                        if (ViewBag.TweetCount > 0)
                        {
                            var model = new AnalyzeViewModel()
                            {
                                MostLikedStatuses = Twitter.GetMostLikedTweets(screenName).Take(10),
                                MostReTweetStatuses = Twitter.GetMostReTweeted(screenName).Take(10)
                            };
                            ViewBag.Words = CountWords(screenName);
                            return View(model);
                        }
                        return View();
                    }
                    screenName = screenName.ToLower();
                    ViewBag.ScreenName = screenName;
                    ViewBag.ImageUrl = Twitter.GetAvatar(screenName);
                    ViewBag.TweetCount = Twitter.GetTweetCount(screenName);
                    ViewBag.FollowersCount = Twitter.GetFollowerCount(screenName);
                    ViewBag.FollowingCount = Twitter.GetFollowingCount(screenName);

                    return View();
                }
                else
                {
                    ViewBag.Hata = "Böyle bir kullanıcı bulunamadı.";
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [ChildActionOnly]
        public IOrderedEnumerable<KeyValuePair<string, int>> CountWords(string screenName)
        {
            var allTweets = Twitter.SearchUserTweet(screenName);

            IList<string> tweets = new List<string>();
            foreach (var tweet in allTweets)
            {
                tweets.Add(tweet.Text);
            }

            var textPath = HostingEnvironment.MapPath("~/tweets.txt");
            if (textPath != null)
            {
                System.IO.File.AppendAllLines(textPath, tweets);


                HashSet<string> words = Word.ExtractWords(System.IO.File.ReadAllText(textPath));
                System.IO.File.Delete(textPath);

                int[] occurrences = new int[words.Count];

                foreach (var tweet in allTweets)
                {
                    int i = 0;
                    foreach (var word in words)
                    {
                        int wordOccurrences = Word.CountOccurrencesIgnoreCase(word, tweet.Text);
                        occurrences[i++] += wordOccurrences;
                    }
                }

                int j = 0;
                Dictionary<string, int> wordDictionary = new Dictionary<string, int>();
                foreach (var word in words)
                {
                    //Response.Write($"{word} --> {occurrences[j++]}");
                    wordDictionary.Add(word, occurrences[j++]);
                }
                return wordDictionary.OrderByDescending(x=>x.Value);
            }
            return null;
        }
    }
}
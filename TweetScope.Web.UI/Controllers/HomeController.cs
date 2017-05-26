using System.Linq;
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
                if (Twitter.GetUser(screenName)!=null)
                {
                    if (Twitter.GetUser(screenName).Protected != true) { 
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
                        return View(model);
                    }
                    return View();
                   }
                    else
                    {
                        screenName = screenName.ToLower();
                        ViewBag.ScreenName = screenName;
                        ViewBag.ImageUrl = Twitter.GetAvatar(screenName);
                        ViewBag.TweetCount = Twitter.GetTweetCount(screenName);
                        ViewBag.FollowersCount = Twitter.GetFollowerCount(screenName);
                        ViewBag.FollowingCount = Twitter.GetFollowingCount(screenName);

                        return View();
                    }
                }
                
                else
                {
                    ViewBag.Hata = "Böyle bir kullanıcı bulunamadı.";
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
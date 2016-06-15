using System;
using CoffeeCups.Utils;
using Xamarin.Forms;
using CoffeeCups.iOS;
using System.Threading.Tasks;
using Foundation;
using Xamarin.Auth;
using UIKit;
using CoffeeCups.AWSBackend;

[assembly: Dependency(typeof(AWSAuthentication))]
namespace CoffeeCups.iOS
{
    public class AWSAuthentication : IAuthentication
    {
        

        public void ClearCookies()
        {
            var store = NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach (var c in cookies)
            {
                store.DeleteCookie(c);
            }

        }

        public async Task<bool> LoginAsync(IDataService client)
        {
            

            var viewController = GetVisibleViewController();
            //Twitter with OAuth1
            var auth = new OAuth1Authenticator(
                consumerKey: AWSConstants.TwitterKey,
                consumerSecret: AWSConstants.TwitterSecret,
                requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
                authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
                accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
                callbackUrl: new Uri("http://motzcod.es")
            );

            auth.Completed += async (sender, eventArgs) =>
            {
                
                if (eventArgs.IsAuthenticated)
                {
                    
                    var token = eventArgs.Account.Properties["oauth_token"];
                    var secret = eventArgs.Account.Properties["oauth_token_secret"];
                    var id = eventArgs.Account.Properties["user_id"];
                    var screen = eventArgs.Account.Properties["screen_name"];

                    Settings.AuthToken = token + ";" + secret;

                    //Store details for future use, 
                    //so we don't have to prompt authentication screen everytime
                    AccountStore.Create().Save(eventArgs.Account, "Twitter");

                }
                await viewController.DismissViewControllerAsync(true);

            };

            await viewController.PresentViewControllerAsync(auth.GetUI(), true);

            return true;
        }


        /// <summary>
        /// Gets the visible view controller.
        /// </summary>
        /// <returns>The visible view controller.</returns>
        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
            {
                return ((UINavigationController)rootController.PresentedViewController).TopViewController;
            }

            if (rootController.PresentedViewController is UITabBarController)
            {
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;
            }

            return rootController.PresentedViewController;
        }
    }
}


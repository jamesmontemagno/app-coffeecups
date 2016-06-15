using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

using CoffeeCups.Droid;
using CoffeeCups.Utils;
using Xamarin.Auth;
using Android.Content;
using CoffeeCups.AWSBackend;

[assembly: Dependency(typeof(AWSAuthentication))]
namespace CoffeeCups.Droid
{
    public class AWSAuthentication : IAuthentication
    {
        public async Task<bool> LoginAsync(IDataService dataService)
        {
            //Twitter with OAuth1
            var auth = new OAuth1Authenticator(
                consumerKey: AWSConstants.TwitterKey,
                consumerSecret: AWSConstants.TwitterSecret,
                requestTokenUrl: new Uri("https://api.twitter.com/oauth/request_token"),
                authorizeUrl: new Uri("https://api.twitter.com/oauth/authorize"),
                accessTokenUrl: new Uri("https://api.twitter.com/oauth/access_token"),
                callbackUrl: new Uri("http://motzcod.es")
            );

            auth.Completed += (sender, eventArgs) =>
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
                    AccountStore.Create(Forms.Context).Save(eventArgs.Account, "Twitter");

                }
            };

            var intent = auth.GetUI(Forms.Context);

            intent.SetFlags(ActivityFlags.ClearTop);
            intent.SetFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);

            return true;

        }

        public void ClearCookies()
        {
            try
            {
                if ((int)global::Android.OS.Build.VERSION.SdkInt >= 21)
                    global::Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch (Exception ex)
            {
            }
        }
    }
}


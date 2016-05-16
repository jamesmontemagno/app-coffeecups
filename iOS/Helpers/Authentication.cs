using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using CoffeeCups.Helpers;
using Foundation;
using Xamarin.Forms;
using CoffeeCups.iOS;

[assembly:Dependency(typeof(Authentication))]
namespace CoffeeCups.iOS
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                var root = window.RootViewController;
                if(root != null)
                {
                    var current = root;
                    while(current.PresentedViewController != null)
                    {
                        current = current.PresentedViewController;
                    }


                    Settings.LoginAttempts++;

                    var user = await client.LoginAsync(current, provider);

                    Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                    Settings.UserId = user?.UserId ?? string.Empty;

                    return user;
                }
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return null;
        }

        public void ClearCookies()
        {
            var store = NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach(var c in cookies)
            {
                store.DeleteCookie(c);
            }
        }
    }
}


using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Foundation;
using Xamarin.Forms;
using CoffeeCups.iOS;
using CoffeeCups.Utils;
using System.Collections.Generic;
using Facebook.LoginKit;

[assembly:Dependency(typeof(AzureAuthentication))]
namespace CoffeeCups.iOS
{
    public class AzureAuthentication : IAuthentication
    {
        public async Task<bool> LoginAsync(IDataService dataService)
        {
            try
            {
                var client = (dataService as AzureService).MobileService;

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

                    var user = await client.LoginAsync(current, MobileServiceAuthenticationProvider.MicrosoftAccount);

                    Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                    Settings.UserId = user?.UserId ?? string.Empty;

                    return true;
                }
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return false;
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


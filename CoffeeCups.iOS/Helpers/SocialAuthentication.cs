
using System;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoffeeCups.Authentication;
using CoffeeCups.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(SocialAuthentication))]
namespace CoffeeCups.iOS.Helpers
{
    public class SocialAuthentication : BaseSocialAuthentication
    {
        public override async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string,string> parameters = null)
        {
            try
            {
                var window = UIKit.UIApplication.SharedApplication.KeyWindow;
                var root = window.RootViewController;
                if (root == null)
                    return null;

                var current = root;
                while (current.PresentedViewController != null)
                {
                    current = current.PresentedViewController;
                }

				return await client.LoginAsync(current, provider, parameters);
                
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return null;
        }



        public override void ClearCookies()
        {
            var store = NSHttpCookieStorage.SharedStorage;
            var cookies = store.Cookies;

            foreach (var c in cookies)
            {
                store.DeleteCookie(c);
            }
        }
	}
}

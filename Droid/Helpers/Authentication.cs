using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using CoffeeCups.Helpers;
using CoffeeCups.Droid;

[assembly:Dependency(typeof(Authentication))]
namespace CoffeeCups.Droid
{
    
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {

                Settings.LoginAttempts++;
                var user = await client.LoginAsync(Forms.Context, provider);
                Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.UserId = user?.UserId ?? string.Empty;
                return user;
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
                Xamarin.Insights.Report(e);
            }

            return null;
        }

        public void ClearCookies()
        {
            try
            {
                if((int)global::Android.OS.Build.VERSION.SdkInt >= 21)
                    global::Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
            }
            catch(Exception ex)
            {
            }
        }
    }
}


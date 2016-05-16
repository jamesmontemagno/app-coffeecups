using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;

using CoffeeCups.Droid;
using CoffeeCups.Utils;

[assembly:Dependency(typeof(AzureAuthentication))]
namespace CoffeeCups.Droid
{
    
    public class AzureAuthentication : IAuthentication
    {
        public async Task<bool> LoginAsync(IDataService dataService)
        {
            try
            {
                var client = (dataService as AzureService).MobileService;
                Settings.LoginAttempts++;
                var user = await client.LoginAsync(Forms.Context, MobileServiceAuthenticationProvider.MicrosoftAccount);
                Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.UserId = user?.UserId ?? string.Empty;
                return true;
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return false;
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


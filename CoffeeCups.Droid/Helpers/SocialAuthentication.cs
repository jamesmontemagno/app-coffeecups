using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms;
using System.Collections.Generic;
using CoffeeCups.Authentication;
using CoffeeCups.Droid.Helpers;

[assembly: Dependency(typeof(SocialAuthentication))]
namespace CoffeeCups.Droid.Helpers
{
    public class SocialAuthentication : BaseSocialAuthentication
    {
        public override async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> parameters = null)
        {
            try
            {
                
                return await client.LoginAsync(Forms.Context, provider, parameters);
 
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return null;
        }

        public override void ClearCookies()
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
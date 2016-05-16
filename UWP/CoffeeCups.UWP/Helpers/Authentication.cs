using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using CoffeeCups.Helpers;
using Xamarin.Forms;
using CoffeeCups.UWP;

[assembly:Dependency(typeof(Authentication))]
namespace CoffeeCups.UWP
{
    public class Authentication : IAuthentication
    {
        public async Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            try
            {
                var user = await client.LoginAsync(provider);

                Settings.AuthToken = user?.MobileServiceAuthenticationToken ?? string.Empty;
                Settings.UserId = user?.UserId ?? string.Empty;

                return user;
            }
            catch(Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return null;
        }

        public void ClearCookies()
        {
            
        }
    }
}


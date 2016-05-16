using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using CoffeeCups.Utils;
using Xamarin.Forms;
using CoffeeCups.UWP;

[assembly:Dependency(typeof(Authentication))]
namespace CoffeeCups.UWP
{
    public class Authentication : IAuthentication
    {
        public async Task<bool> LoginAsync(IDataService dataService)
        {
            try
            {
                var client = (dataService as AzureService).MobileService;
                var user = await client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);

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
            
        }
    }
}


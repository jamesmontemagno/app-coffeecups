#define AZURE

#if AZURE

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using CoffeeCups.Helpers;
using System.Diagnostics;

namespace CoffeeCups.Authentication
{
    public abstract class BaseSocialAuthentication : IAuthentication
    {
        public virtual void ClearCookies()
        {
            
        }

        public abstract Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> paramameters = null);


        public virtual async Task<bool> RefreshUser(IMobileServiceClient client)
        {
            try
            {
                var user = await client.RefreshUserAsync();

                if (user != null)
                {
                    client.CurrentUser = user;
                    Settings.AuthToken = user.MobileServiceAuthenticationToken;
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine("Unable to refresh user: " + e);
            }

            return false;
        }
    }
}
#endif
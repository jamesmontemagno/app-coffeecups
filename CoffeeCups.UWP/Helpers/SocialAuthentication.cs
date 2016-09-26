using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using CoffeeCups.Authentication;
using CoffeeCups.UWP.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(SocialAuthentication))]
namespace CoffeeCups.UWP.Helpers
{
    public class SocialAuthentication : BaseSocialAuthentication
    {
        public override async Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string,string> parameters = null)
        {
            try
            {
				return await client.LoginAsync(provider, parameters);
                
            }
            catch (Exception e)
            {
                e.Data["method"] = "LoginAsync";
            }

            return null;
        }
	}
}
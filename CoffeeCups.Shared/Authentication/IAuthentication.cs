#define AZURE

#if AZURE
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoffeeCups.Authentication
{
    public interface IAuthentication
    {
		Task<MobileServiceUser> LoginAsync(IMobileServiceClient client, MobileServiceAuthenticationProvider provider, IDictionary<string, string> paramameters = null);
		Task<bool> RefreshUser(IMobileServiceClient client);
        void ClearCookies();
    }
}
#endif

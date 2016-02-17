using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace CoffeeCups
{
    public interface IAuthentication
    {
        Task<MobileServiceUser> LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider);
        void ClearCookies();
    }
}


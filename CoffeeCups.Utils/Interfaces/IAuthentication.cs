using System;
using System.Threading.Tasks;
using CoffeeCups.DataObjects;

namespace CoffeeCups.Utils
{
    public enum AuthProvider
    {
        Facebook,
        Microsoft
    }

    public interface IAuthentication
    {
        Task<bool> LoginAsync(IDataService client);
        void ClearCookies();
    }
}


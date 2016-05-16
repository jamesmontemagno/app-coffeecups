using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoffeeCups.DataObjects;

namespace CoffeeCups.Utils
{
    public interface IDataService
    {

        Task InitializeAsync();

        Task<IEnumerable<CupOfCoffee>> GetAllCoffeeAsync();

        Task<CupOfCoffee> AddCoffeeAsync(bool atHome, string os);

        Task SyncCoffeeAsync();

        bool NeedsAuthentication { get; }

        AuthProvider AuthProvider { get; }
    }
}


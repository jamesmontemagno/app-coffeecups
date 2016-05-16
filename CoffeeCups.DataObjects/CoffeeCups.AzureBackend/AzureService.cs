using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using CoffeeCups.Utils;
using CoffeeCups.DataObjects;

namespace CoffeeCups
{
    public class AzureService : IDataService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable<CupOfCoffee> coffeeTable;

        bool isInitialized;
        public async Task InitializeAsync()
        {
            if (isInitialized)
                return;

            

            var handler = new AuthHandler();
            //Create our client
            MobileService = new MobileServiceClient("https://mycoffeeapp.azurewebsites.net", handler);
            handler.Client = MobileService;

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                MobileService.CurrentUser = new MobileServiceUser (Settings.UserId);
                MobileService.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
            
            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            store.DefineTable<CupOfCoffee>();

            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            coffeeTable = MobileService.GetSyncTable<CupOfCoffee>();

            isInitialized = true;
        }

        public async Task<IEnumerable<CupOfCoffee>> GetAllCoffeeAsync()
        {
            await InitializeAsync();
            await SyncCoffeeAsync();
            return await coffeeTable.OrderBy(c => c.DateUtc).ToEnumerableAsync();
        }

        public async Task<CupOfCoffee> AddCoffeeAsync(bool atHome, string os)
        {
            await InitializeAsync();

            //create and insert coffee
            var coffee = new CupOfCoffee
            {
                    DateUtc = DateTime.UtcNow,
                    MadeAtHome = atHome,
                    OS = os
            };

            await coffeeTable.InsertAsync(coffee);

            //Synchronize coffee
            await SyncCoffeeAsync();

            return coffee;
        }

        public async Task SyncCoffeeAsync()
        {
            try
            {
                //pull down all latest changes and then push current coffees up
                await coffeeTable.PullAsync("allCoffees", coffeeTable.CreateQuery());
                await MobileService.SyncContext.PushAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Unable to sync coffees, that is alright as we have offline capabilities: " + ex);
            }
        }

        public bool NeedsAuthentication => true;

        public AuthProvider AuthProvider => AuthProvider.Microsoft;
    }
}
    

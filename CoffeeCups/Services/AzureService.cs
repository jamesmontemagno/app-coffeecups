using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using Xamarin.Forms;
using CoffeeCups.Helpers;

namespace CoffeeCups
{
    public class AzureService
    {
        public MobileServiceClient MobileService { get; set; }
        IMobileServiceSyncTable<CupOfCoffee> coffeeTable;

        bool isInitialized;
        public async Task Initialize()
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

        public async Task<IEnumerable<CupOfCoffee>> GetCoffees()
        {
            await Initialize();
            await SyncCoffee();
            return await coffeeTable.OrderBy(c => c.DateUtc).ToEnumerableAsync();
        }

        public async Task<CupOfCoffee> AddCoffee(bool atHome)
        {
            await Initialize();

            //create and insert coffee
            var coffee = new CupOfCoffee
            {
                    DateUtc = DateTime.UtcNow,
                    MadeAtHome = atHome,
                    OS = Device.OS.ToString()
            };

            await coffeeTable.InsertAsync(coffee);

            //Synchronize coffee
            await SyncCoffee();

            return coffee;
        }

        public async Task SyncCoffee()
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
    }
}
    

using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using Xamarin.Forms;

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

            var time = Xamarin.Insights.TrackTime("InitializeTime");
            time.Start();
            
            //Create our client
            MobileService = new MobileServiceClient("https://mycoffeeapp.azurewebsites.net");

            const string path = "syncstore.db";
            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            store.DefineTable<CupOfCoffee>();

            await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            coffeeTable = MobileService.GetSyncTable<CupOfCoffee>();

            isInitialized = true;
            time.Stop();
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

            var time = Xamarin.Insights.TrackTime("AddCoffeeTime");
            time.Start();
            //create and insert coffee
            var coffee = new CupOfCoffee
            {
                    DateUtc = DateTime.UtcNow,
                    MadeAtHome = atHome,
                    OS = Device.OS.ToString()
            };

            await coffeeTable.InsertAsync(coffee);
            time.Stop();

            //Synchronize coffee
            await SyncCoffee();

            return coffee;
        }

        public async Task SyncCoffee()
        {
            var time = Xamarin.Insights.TrackTime("SyncCoffeeTime");
            time.Start();
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

            time.Stop();
        }
    }
}
    
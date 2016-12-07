//#define AUTH

using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using Xamarin.Forms;
using CoffeeCups.Helpers;
using CoffeeCups.Authentication;
using CoffeeCups;
using System.IO;
using Plugin.Connectivity;



[assembly: Dependency(typeof(AzureService))]
namespace CoffeeCups
{
    public class AzureService
    {

        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<CupOfCoffee> coffeeTable;
        public static bool UseAuth { get; set; } = false;
 
        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;


            var appUrl = "https://YOUR-BACKEND-URL-HERE.azurewebsites.net";

#if AUTH      
            Client = new MobileServiceClient(appUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                Client.CurrentUser = new MobileServiceUser (Settings.UserId);
                Client.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
#else
            //Create our client

            Client = new MobileServiceClient(appUrl);

#endif

            //InitialzeDatabase for path
            var path = "syncstore.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<CupOfCoffee>();


            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store);

            //Get our sync table that will call out to azure
            coffeeTable = Client.GetSyncTable<CupOfCoffee>();

            
        }

        public async Task SyncCoffee()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                    return;

                await coffeeTable.PullAsync("allCoffee", coffeeTable.CreateQuery());

                await Client.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync coffees, that is alright as we have offline capabilities: " + ex);
            }

        }

        public async Task<IEnumerable<CupOfCoffee>> GetCoffees()
        {
            //Initialize & Sync
            await Initialize();
            await SyncCoffee();

            return await coffeeTable.OrderBy(c => c.DateUtc).ToEnumerableAsync(); ;
            
        }

        public async Task<CupOfCoffee> AddCoffee(bool atHome)
        {
            await Initialize();

            var coffee = new CupOfCoffee
            {
                DateUtc = DateTime.UtcNow,
                MadeAtHome = atHome,
                OS = Device.OS.ToString()
            };

            await coffeeTable.InsertAsync(coffee);

            await SyncCoffee();
            //return coffee
            return coffee;
        }

      

        public async Task<bool> LoginAsync()
        {

            await Initialize();

            var auth = DependencyService.Get<IAuthentication>();
            var user = await auth.LoginAsync(Client, MobileServiceAuthenticationProvider.Twitter);

            if (user == null)
            {
                Settings.AuthToken = string.Empty;
                Settings.UserId = string.Empty;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Login Error", "Unable to login, please try again", "OK");
                });
                return false;
            }
            else
            {
                Settings.AuthToken = user.MobileServiceAuthenticationToken;
                Settings.UserId = user.UserId;
            }

            return true;
        }
    }
}
    

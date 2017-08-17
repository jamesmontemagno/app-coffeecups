#define AUTH

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
using Newtonsoft.Json.Linq;

[assembly: Dependency(typeof(AzureService))]
namespace CoffeeCups
{
    public class AzureService
    {

        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<CupOfCoffee> coffeeTable;

#if AUTH
        public static bool UseAuth { get; set; } = true;
#else
        public static bool UseAuth { get; set; } = false;
#endif


        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;


            var appUrl = "https://ilovecoffee.azurewebsites.net";

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

            var path = "mydatabase2.db";
            var store = new MobileServiceSQLiteStore(path);

            //setup our local sqlite store and intialize our table

            
            store.DefineTable<CupOfCoffee>();

            //Define table



            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store);

            //Get our sync table that will call out to azure
            coffeeTable = Client.GetSyncTable<CupOfCoffee>();


            
        }

        public async Task SyncCoffee()
        {
            try
            {
               if(!CrossConnectivity.Current.IsConnected)
                {
                    return;
                }

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

            return await coffeeTable.OrderByDescending(c => c.DateUtc).ToEnumerableAsync();
            
        }

        public async Task<CupOfCoffee> AddCoffee(bool atHome, string location)
        {
            await Initialize();

            var coffee = new CupOfCoffee
            {
                DateUtc = DateTime.UtcNow,
                MadeAtHome = atHome,
                OS = Device.RuntimePlatform,
                Location = location ?? string.Empty
            };

            await coffeeTable.InsertAsync(coffee);

            await SyncCoffee();

          
           
            return coffee;
        }

      

        public async Task<bool> LoginAsync()
        {

            await Initialize();

            var provider = MobileServiceAuthenticationProvider.Twitter;
            var uriScheme = "coffeecups";


#if __ANDROID__
            //Android we are using facebook:
            var token = new JObject();
            token["access_token"] = Settings.FacebookAccessToken;
            var user = await Client.LoginAsync(MobileServiceAuthenticationProvider.Facebook, token);

#elif __IOS__
            CoffeeCups.iOS.AppDelegate.ResumeWithURL = url => url.Scheme == uriScheme && Client.ResumeWithURL(url);
            var user = await Client.LoginAsync(GetController(), provider, uriScheme);
            
#else
            var user = await Client.LoginAsync(provider, uriScheme);
            
#endif
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


#if __IOS__
         UIKit.UIViewController GetController()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;
            if (root == null)
                return null;

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }

            return current;
        }
#endif
    }
}
    

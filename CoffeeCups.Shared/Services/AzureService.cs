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
            //check to see if the SyncContext is Initialized
            


            var appUrl = "https://URL-HERE.azurewebsites.net";

#if AUTH      
            Client = new MobileServiceClient(appUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                Client.CurrentUser = new MobileServiceUser (Settings.UserId);
                Client.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
#else
            //Create our client
            

#endif

           
            
            //setup our local sqlite store and intialize our table

            //Define tables


            //Initialize SyncContext

            //Get our sync table that will call out to azure

            
        }

        public async Task SyncCoffee()
        {
            try
            {
                //Check Connectivity and then try to sync.


                //Pull latest coffee from server


                //Push any local changes up to server.

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to sync coffees, that is alright as we have offline capabilities: " + ex);
            }

        }

        public async Task<IEnumerable<CupOfCoffee>> GetCoffees()
        {
            //Initialize
            await Initialize();

            //Sync Coffee

            //Query coffees from the table.
            return null;
            
        }

        public async Task<CupOfCoffee> AddCoffee(bool atHome)
        {
            await Initialize();

            //Create a coffee to insert and sync


            //Insert coffe LOCALLY into coffee table


            //Attempt to sync coffee to the server


            //Return the coffee that we just created.
            return null;
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
    

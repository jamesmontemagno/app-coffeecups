using System;
using MvvmHelpers;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using FormsToolkit;
using System.Linq;
using CoffeeCups.Helpers;
using Microsoft.WindowsAzure.MobileServices;

namespace CoffeeCups
{
    public class CoffeesViewModel : BaseViewModel
    {
        AzureService azureService;
        public CoffeesViewModel()
        {
            azureService = new AzureService();
        }
           
        public ObservableRangeCollection<CupOfCoffee> Coffees { get; } = new ObservableRangeCollection<CupOfCoffee>();
        public ObservableRangeCollection<Grouping<string, CupOfCoffee>> CoffeesGrouped { get; } = new ObservableRangeCollection<Grouping<string, CupOfCoffee>>();

        string loadingMessage;
        public string LoadingMessage
        {
            get { return loadingMessage; }
            set { SetProperty(ref loadingMessage, value); }
        }

        ICommand  loadCoffeesCommand;
        public ICommand LoadCoffeesCommand =>
            loadCoffeesCommand ?? (loadCoffeesCommand = new Command(async () => await ExecuteLoadCoffeesCommandAsync())); 

        async Task ExecuteLoadCoffeesCommandAsync()
        {
            if(IsBusy || !Settings.IsLoggedIn)
                return;


            try 
            {
                
                LoadingMessage = "Loading Coffees...";
                IsBusy = true;
                var coffees = await azureService.GetCoffees();
                Coffees.ReplaceRange(coffees);


                SortCoffees();


            }
            catch (Exception ex) 
            {
                Debug.WriteLine("OH NO!" + ex);
                MessagingService.Current.SendMessage<MessagingServiceAlert>("message", new MessagingServiceAlert
                    {
                        Cancel ="OK",
                        Message="Unable to sync coffees, you may be offline",
                        Title ="Coffee sync Error"
                    });
            } 
            finally 
            {
                IsBusy = false;
            }


        }

        void SortCoffees()
        {
            var groups = from coffee in Coffees
                orderby coffee.DateUtc descending
                group coffee by coffee.DateDisplay
                into coffeeGroup
                select new Grouping<string, CupOfCoffee>($"{coffeeGroup.Key} ({coffeeGroup.Count()})", coffeeGroup);


            CoffeesGrouped.ReplaceRange(groups);
        }

        bool atHome;
        public bool AtHome
        {
            get { return atHome; }
            set { SetProperty(ref atHome, value); }
        }

        ICommand  addCoffeeCommand;
        public ICommand AddCoffeeCommand =>
            addCoffeeCommand ?? (addCoffeeCommand = new Command(async () => await ExecuteAddCoffeeCommandAsync())); 

        async Task ExecuteAddCoffeeCommandAsync()
        {
            if(IsBusy)
                return;

            try 
            {

                if(!Settings.IsLoggedIn)
                {
                    await azureService.Initialize();
                    var user = await DependencyService.Get<IAuthentication>().LoginAsync(azureService.MobileService, MobileServiceAuthenticationProvider.MicrosoftAccount);
                    if(user == null)
                        return;

                    var coffees = await azureService.GetCoffees();
                    Coffees.ReplaceRange(coffees);

                    SortCoffees();
                }
                LoadingMessage = "Adding Coffee...";
                IsBusy = true;
                Xamarin.Insights.Track("CoffeeAdded");

                var coffee = await azureService.AddCoffee(AtHome);
                Coffees.Add(coffee);
                SortCoffees();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("OH NO!" + ex);
                //This is okay because we can 
                Xamarin.Insights.Report(ex);
            } 
            finally 
            {
                IsBusy = false;
            }

        }
    }
}


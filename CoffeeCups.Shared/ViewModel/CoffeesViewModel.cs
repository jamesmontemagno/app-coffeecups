using System;
using MvvmHelpers;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using System.Linq;
using CoffeeCups.Helpers;

namespace CoffeeCups
{
    public class CoffeesViewModel : BaseViewModel
    {
        AzureService azureService;
        public CoffeesViewModel()
        {
            azureService = DependencyService.Get<AzureService>();
        }
           
        public ObservableRangeCollection<CupOfCoffee> Coffees { get; } = new ObservableRangeCollection<CupOfCoffee>();
        public ObservableRangeCollection<Grouping<string, CupOfCoffee>> CoffeesGrouped { get; } = new ObservableRangeCollection<Grouping<string, CupOfCoffee>>();

        string loadingMessage;
        public string LoadingMessage
        {
            get { return loadingMessage; }
            set { SetProperty(ref loadingMessage, value); }
        }


        bool atHome;
        public bool AtHome
        {
            get => atHome;
            set => SetProperty(ref atHome, value);
        }


        string location;
        public string Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }


        ICommand loadCoffeesCommand;
        public ICommand LoadCoffeesCommand =>
            loadCoffeesCommand ?? (loadCoffeesCommand = new Command(async () => await ExecuteLoadCoffeesCommandAsync())); 

        async Task ExecuteLoadCoffeesCommandAsync()
        {
            if(IsBusy || !(await LoginAsync()))
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

                await Application.Current.MainPage.DisplayAlert("Sync Error", "Unable to sync coffees, you may be offline", "OK");
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

        ICommand  addCoffeeCommand;
        public ICommand AddCoffeeCommand =>
            addCoffeeCommand ?? (addCoffeeCommand = new Command(async () => await ExecuteAddCoffeeCommandAsync())); 

        async Task ExecuteAddCoffeeCommandAsync()
        {
            if(IsBusy || !(await LoginAsync()))
                return;

            try
            {

                if (string.IsNullOrWhiteSpace(Location) && !AtHome)
                {
                    await Application.Current.MainPage.DisplayAlert("Needs Location", "Please enter a location before adding the coffee.", "OK");
                    return;
                }
                LoadingMessage = "Adding Coffee...";
                IsBusy = true;
                

                var coffee = await azureService.AddCoffee(AtHome, location);
                Location = string.Empty;
                AtHome = false;
                Coffees.Add(coffee);
                SortCoffees();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("OH NO!" + ex);
            } 
            finally 
            {
                LoadingMessage = string.Empty;
                IsBusy = false;
            }

        }

        public Task<bool> LoginAsync()
        {
            if (Settings.IsLoggedIn)
                return Task.FromResult(true);


            return azureService.LoginAsync();
        }
    }
}


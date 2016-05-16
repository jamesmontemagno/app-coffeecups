using System;
using MvvmHelpers;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using FormsToolkit;
using System.Linq;
using CoffeeCups.Utils;
using CoffeeCups.DataObjects;

namespace CoffeeCups
{
    public class CoffeesViewModel : BaseViewModel
    {
        IDataService coffeeService;
        public CoffeesViewModel()
        {

            coffeeService = new AWSBackend.AWSService();
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
            if(IsBusy)
                return;

            try 
            {
                if(!Settings.IsLoggedIn && coffeeService.NeedsAuthentication)
                {
                    await coffeeService.InitializeAsync();
                    await DependencyService.Get<IAuthentication>().LoginAsync(coffeeService);
                }
                
                LoadingMessage = "Loading Coffees...";
                IsBusy = true;
                var coffees = await coffeeService.GetAllCoffeeAsync();
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

                if (!Settings.IsLoggedIn && coffeeService.NeedsAuthentication)
                {
                    await coffeeService.InitializeAsync();

                    var user = await DependencyService.Get<IAuthentication>().LoginAsync(coffeeService);
                    if (user == null)
                        return;

                    LoadingMessage = "Adding Coffee...";
                    IsBusy = true;

                    var coffees = await coffeeService.GetAllCoffeeAsync();
                    Coffees.ReplaceRange(coffees);

                    SortCoffees();
                }
                else
                {
                    LoadingMessage = "Adding Coffee...";
                    IsBusy = true;
                }

                var coffee = await coffeeService.AddCoffeeAsync(AtHome, Device.OS.ToString());
                Coffees.Add(coffee);
                SortCoffees();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("OH NO!" + ex);
                //This is okay because we can 
            } 
            finally 
            {
                IsBusy = false;
            }

        }
    }
}


using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Plugin.Connectivity;

namespace CoffeeCups
{
    public partial class CoffeesPage : ContentPage
    {
        CoffeesViewModel vm;
        public CoffeesPage()
        {
            InitializeComponent();
            BindingContext = vm = new CoffeesViewModel();
            ListViewCoffees.ItemTapped += (sender, e) => ListViewCoffees.SelectedItem = null;;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

             CrossConnectivity.Current.ConnectivityChanged += ConnecitvityChanged;
            OfflineStack.IsVisible = !CrossConnectivity.Current.IsConnected;
            if (vm.Coffees.Count == 0)
                vm.LoadCoffeesCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            CrossConnectivity.Current.ConnectivityChanged -= ConnecitvityChanged;
        }

        void ConnecitvityChanged (object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
                {
                    OfflineStack.IsVisible = !e.IsConnected;
                });
        }
    }
}


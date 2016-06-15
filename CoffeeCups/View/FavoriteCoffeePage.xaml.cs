
using System;
using System.Collections.Generic;
using CoffeeCups.Utils;
using Xamarin.Forms;

namespace CoffeeCups
{
    public partial class FavoriteCoffeePage : ContentPage
    {
        AWSBackend.AWSFavoriteService favoriteService;
        public FavoriteCoffeePage()
        {
            InitializeComponent();
        }

        async void SaveClicked(object sender, System.EventArgs e)
        {

            if (favoriteService == null)
                favoriteService = new AWSBackend.AWSFavoriteService();
            
            IndicatorIsBusy.IsRunning = true;
            await favoriteService.SaveFavAsync("fav", EntryFavorite.Text);
            IndicatorIsBusy.IsRunning = false;
        }

        async void SyncClicked(object sender, System.EventArgs e)
        {
            if (favoriteService == null)
                favoriteService = new AWSBackend.AWSFavoriteService();

            IndicatorIsBusy.IsRunning = true;
            EntryFavorite.Text = await favoriteService.GetAsync("fav");
            IndicatorIsBusy.IsRunning = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var login = DependencyService.Get<IAuthentication>();

            if (string.IsNullOrWhiteSpace(Settings.AuthToken))
            {
                await login.LoginAsync(null);
            }

        }

    }
}


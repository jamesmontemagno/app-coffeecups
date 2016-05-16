
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace CoffeeCups
{
    public partial class FavoriteCoffeePage : ContentPage
    {
        AWSBackend.AWSFavoriteService favoriteService;
        public FavoriteCoffeePage()
        {
            InitializeComponent();
            favoriteService = new AWSBackend.AWSFavoriteService();
        }

        async void SaveClicked(object sender, System.EventArgs e)
        {
            IndicatorIsBusy.IsRunning = true;
            await favoriteService.SaveFavAsync("fav", EntryFavorite.Text);
            IndicatorIsBusy.IsRunning = false;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            IndicatorIsBusy.IsRunning = true;
            EntryFavorite.Text = await favoriteService.GetAsync("fav");
            IndicatorIsBusy.IsRunning = false;
        }

    }
}


using System;

using Xamarin.Forms;
using FormsToolkit;

namespace CoffeeCups
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = new NavigationPageNoLine(new CoffeesPage())
                {
                    BarTextColor = Color.White,
                    BarBackgroundColor = Color.FromHex("#F2C500")
                };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            MessagingService.Current.Subscribe<MessagingServiceAlert>("message", async (m, info) =>
                {
                    var task = Application.Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);

                    if(task == null)
                        return;  

                    await task;
                    info?.OnCompleted?.Invoke();
                });
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}


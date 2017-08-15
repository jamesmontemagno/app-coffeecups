using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;

namespace CoffeeCups.Droid
{
	[Activity (Label = "Coffee Cups", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{

            ToolbarResource = Resource.Layout.Toolbar;
            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate (bundle);
			global::Xamarin.Forms.Forms.Init (this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            LoadApplication (new App ());
		}
	}
}

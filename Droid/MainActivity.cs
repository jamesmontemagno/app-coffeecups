using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;

namespace CoffeeCups.Droid
{
	[Activity (Label = "CoffeeCups", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
	{
		protected override void OnCreate (Bundle bundle)
		{

            FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
            FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;
            Xamarin.Insights.Initialize ("4c9b7ee70e74ac06196f21599c67983461959028", this);
            Xamarin.Insights.ForceDataTransmission = true;
            base.OnCreate (bundle);
			global::Xamarin.Forms.Forms.Init (this, bundle);

            FormsToolkit.Droid.Toolkit.Init();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			LoadApplication (new App ());
		}
	}
}

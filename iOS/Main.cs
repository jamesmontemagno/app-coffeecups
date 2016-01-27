using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;

namespace CoffeeCups.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
            Xamarin.Insights.Initialize ("4c9b7ee70e74ac06196f21599c67983461959028");
            Xamarin.Insights.ForceDataTransmission = true;
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

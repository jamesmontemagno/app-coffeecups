using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Xamarin.Facebook.Login;
using Xamarin.Facebook;
using Android.Content;
using Android.Runtime;

namespace CoffeeCups.Droid
{
	[Activity (Label = "Coffee Cups", Icon = "@drawable/icon",
        Name = "com.refractored.coffeecups.MainActivity",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
	{
        ICallbackManager callbackManager;

        protected override void OnCreate (Bundle bundle)
        {

            ToolbarResource = Resource.Layout.Toolbar;
            TabLayoutResource = Resource.Layout.Tabbar;

            base.OnCreate (bundle);
	        global::Xamarin.Forms.Forms.Init (this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            FacebookSdk.SdkInitialize(ApplicationContext);
            callbackManager = CallbackManagerFactory.Create();

            var loginCallback = new FacebookCallback<LoginResult>
            {
                HandleSuccess = loginResult => {

                    CoffeeCups.Helpers.Settings.FacebookAccessToken = AccessToken.CurrentAccessToken.Token;
                    App.GoToCoffee();
                },
                HandleCancel = () => {
                  
                },
                HandleError = loginError => {
                  
                }
            };

            LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);

            LoadApplication (new App ());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
        {
            public Action HandleCancel { get; set; }
            public Action<FacebookException> HandleError { get; set; }
            public Action<TResult> HandleSuccess { get; set; }

            public void OnCancel()
            {
                HandleCancel?.Invoke();
            }

            public void OnError(FacebookException error)
            {
                HandleError?.Invoke(error);
            }

            public void OnSuccess(Java.Lang.Object result)
            {
                HandleSuccess?.Invoke(result.JavaCast<TResult>());
            }
        }
    }
}

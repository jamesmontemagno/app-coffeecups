using CoffeeCups.View;
using Xamarin.Forms;
using CoffeeCups.Droid;
using Xamarin.Forms.Platform.Android;
using Xamarin.Facebook.Login.Widget;

[assembly: ExportRenderer(typeof(FacebookLoginButton), typeof(FacebookLoginButtonRenderer))]
namespace CoffeeCups.Droid
{
    public class FacebookLoginButtonRenderer : ViewRenderer<FacebookLoginButton, LoginButton>
    {
        LoginButton facebookLoginButton;
        protected override void OnElementChanged(ElementChangedEventArgs<FacebookLoginButton> e)
        {
            base.OnElementChanged(e);
            if(Control == null || facebookLoginButton == null)
            {
                facebookLoginButton = new LoginButton(Forms.Context);
                SetNativeControl(facebookLoginButton);
            }
        }
        
    }
}
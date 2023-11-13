using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Microsoft.Identity.Client;

namespace MauiAppTutorial
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            PlatformConfig.Instance.RedirectUri = PublicClientSingleton.Instance.MSALClientHelper.AzureADConfig.AndroidRedirectUri;
            PlatformConfig.Instance.ParentWindow = this;
            IAccount existinguser = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.InitializePublicClientAppForWAMBrokerAsync()).Result;

        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }

    }

}
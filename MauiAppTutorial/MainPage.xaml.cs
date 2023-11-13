using Microsoft.Identity.Client;

namespace MauiAppTutorial
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            IAccount cachedUserAccount = Task.Run(async () => await PublicClientSingleton.Instance.MSALClientHelper.FetchSignedInUserFromCache()).Result;

            _ = Application.Current.Dispatcher.Dispatch(async () =>
            {
                if (cachedUserAccount != null)
                {
                
                    await Shell.Current.GoToAsync("userview");
                }
            });

        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {           
            try
            {
                await PublicClientSingleton.Instance.AcquireTokenSilentAsync();
            }
            catch (MsalClientException ex) when (ex.ErrorCode == MsalError.AuthenticationCanceledError)
            {
                await ShowMessage("Login failed", "User cancelled sign in.");
                return;
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS65004"))
            {
                await ShowMessage("Login failed", "User did not consent to app requirements.");
                return;
            }
            await Shell.Current.GoToAsync("userview");
        }
        private async Task ShowMessage(string title, string message)
        {
            _ = this.Dispatcher.Dispatch(async () =>
            {
                await DisplayAlert(title, message, "OK").ConfigureAwait(false);
            });
        }
    }
}
using Microsoft.Identity.Client;

namespace MauiAppTutorial;

public partial class UserView : ContentPage
{
	public  UserView()
	{
        InitializeComponent();
        _ = GetUserInformationAsync();
    }
    private async Task GetUserInformationAsync()
    {
        try
        {
            var user = await PublicClientSingleton.Instance.MSGraphHelper.GetMeAsync();
            UserImage.Source = ImageSource.FromStream(async _ => await PublicClientSingleton.Instance.MSGraphHelper.GetMyPhotoAsync());    
            DisplayName.Text = user.DisplayName;
            Email.Text = user.Mail;
        }
        catch (MsalUiRequiredException)
        {
            await PublicClientSingleton.Instance.SignOutAsync();            
            await Shell.Current.GoToAsync("mainview");
        }
    }
    private async void SignOutButton_Clicked(object sender, EventArgs e)
    {  
        await PublicClientSingleton.Instance.SignOutAsync();

        await Shell.Current.GoToAsync("mainpage");
    }
}
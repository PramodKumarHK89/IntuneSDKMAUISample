using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTutorial
{
    public class MsalClientHelper
    {
        public IPublicClientApplication PublicClientApplication { get; private set; }
        private PublicClientApplicationBuilder PublicClientApplicationBuilder;
        public AzureADConfig AzureADConfig;

        public MsalClientHelper(AzureADConfig azureADConfig)
        {
            AzureADConfig = azureADConfig;
            this.InitializePublicClientApplicationBuilder();

        }
        private void InitializePublicClientApplicationBuilder()
        {
            this.PublicClientApplicationBuilder = PublicClientApplicationBuilder.Create(AzureADConfig.ClientId)
                .WithAuthority(string.Format(AzureADConfig.Authority, AzureADConfig.TenantId))
                .WithExperimentalFeatures() // this is for upcoming logger
            //.WithLogging(new IdentityLogger(EventLogLevel.Warning), enablePiiLogging: false)    // This is the currently recommended way to log MSAL message. For more info refer to https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/logging. Set Identity Logging level to Warning which is a middle ground
                .WithClientCapabilities(new string[] { "cp1" })                                     // declare this client app capable of receiving CAE events- https://aka.ms/clientcae

                .WithIosKeychainSecurityGroup("com.microsoft.adalcache");
        }
       
        private async Task<IEnumerable<IAccount>> AttachTokenCache()
        {            
            // If the cache file is being reused, we'd find some already-signed-in accounts
            return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
        }
        public async Task<IAccount> FetchSignedInUserFromCache()
        {

            if (this.PublicClientApplication == null)
            {
                InitializePublicClientAppForWAMBrokerAsync();
            }
            // get accounts from cache
            IEnumerable<IAccount> accounts = await this.PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
            var acc= accounts.SingleOrDefault();
            if (acc!=null)
            {


#if ANDROID
                CustomMAMManager.Enroll(acc.Username,acc.HomeAccountId.ObjectId, acc.HomeAccountId.TenantId);
#endif
            }
            return acc;
        }
        public async Task<string> SignInUserAndAcquireAccessToken(string[] scopes)
        {
            AuthenticationResult AuthResult;
            var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

            try
            {
                // 1. Try to sign-in the previously signed-in account
                if (existingUser != null)
                {
                    AuthResult = await this.PublicClientApplication
                        .AcquireTokenSilent(scopes, existingUser)
                        .ExecuteAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    AuthResult = await SignInUserInteractivelyAsync(scopes, existingUser);
                }
            }
            catch (MsalUiRequiredException ex)
            {
                // A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token interactively
                //Debug.WriteLine($"MsalUiRequiredException: {ex.Message}");

                AuthResult = await SignInUserInteractivelyAsync(scopes, existingUser);
            }
            catch (MsalException msalEx)
            {
               // Debug.WriteLine($"Error Acquiring Token interactively:{Environment.NewLine}{msalEx}");
                throw msalEx;
            }
            await FetchSignedInUserFromCache().ConfigureAwait(false);
            return AuthResult.AccessToken;
        }
        public async Task<AuthenticationResult> SignInUserInteractivelyAsync(string[] scopes, IAccount existingAccount = null)
        {

            if (this.PublicClientApplication == null)
                throw new NullReferenceException();


            SystemWebViewOptions systemWebViewOptions = new SystemWebViewOptions();
#if IOS
            // Hide the privacy prompt in iOS
            systemWebViewOptions.iOSHidePrivacyPrompt = true;
#endif
            return await this.PublicClientApplication
                    .AcquireTokenInteractive(scopes)
                    .WithLoginHint(existingAccount?.Username ?? String.Empty)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
        }
        public async Task<IAccount> InitializePublicClientAppForWAMBrokerAsync()
        {
            // Initialize the MSAL library by building a public client application
            this.PublicClientApplication = this.PublicClientApplicationBuilder
                .WithRedirectUri(PlatformConfig.Instance.RedirectUri)   // redirect URI is set later in PlatformConfig when the platform is decided
                .WithBroker()
                .WithParentActivityOrWindow(() => PlatformConfig.Instance.ParentWindow)   // This is required when using the WAM broker and is set later in PlatformConfig when the platform has been decided
                .Build();

          //  this.IsBrokerInitialized = true;

             await AttachTokenCache();
            return await FetchSignedInUserFromCache().ConfigureAwait(false);
        }
        public async Task SignOutUserAsync()
        {
            var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);
            await this.SignOutUserAsync(existingUser).ConfigureAwait(false);
        }
        /// <summary>
        /// Removes a given user's record from token cache
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task SignOutUserAsync(IAccount user)
        {
            if (this.PublicClientApplication == null) return;

#if ANDROID
                CustomMAMManager.UnEnroll(user.Username,user.HomeAccountId.ObjectId);
#endif
            await this.PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
        }

    }
}

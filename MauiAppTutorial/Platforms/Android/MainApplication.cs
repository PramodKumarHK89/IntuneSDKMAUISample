using Android.App;
using Android.Runtime;
using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Client.Notification;
using Microsoft.Intune.Mam.Policy;
using Microsoft.Intune.Mam.Policy.Notification;

namespace MauiAppTutorial
{
    [Application]
    public class MainApplication : MauiApplication
    {
        TaskCompletionSource<bool> _enrollmentTask;
        TaskCompletionSource<bool> _unenrollmentTask;

        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            mgr.RegisterAuthenticationCallback(new MAMWEAuthCallback());

            IMAMNotificationReceiverRegistry registry = MAMComponents.Get<IMAMNotificationReceiverRegistry>();
            foreach (MAMNotificationType notification in MAMNotificationType.Values())
            {
                registry.RegisterReceiver(new ToastNotificationReceiver(this), notification);
            }
            registry.RegisterReceiver(new EnrollmentNotificationReceiver(this), MAMNotificationType.MamEnrollmentResult);
            registry.RegisterReceiver(new WipeNotificationReceiver(this), MAMNotificationType.WipeUserData);

            base.OnCreate();
        }     

    }

    public class LoginResponse
    {
        public string UserName { get; set; }
        public string AccountId { get; set; }
        public string TenantId { get; set; }
        public string Token { get; internal set; }
    }
}
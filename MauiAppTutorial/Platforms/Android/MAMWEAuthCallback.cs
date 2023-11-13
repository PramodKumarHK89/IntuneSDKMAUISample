using Android.Util;
using Java.Net;
using Microsoft.Intune.Mam.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTutorial
{
    public class MAMWEAuthCallback : Java.Lang.Object, IMAMServiceAuthenticationCallback
    {
        public string AcquireToken(string upn, string aadId, string resourceId)
        {
            resourceId = "https://wip.mam.manage.microsoft.us//DeviceManagementManagedApps.ReadWrite";
            Task<string> token = PublicClientSingleton.Instance.AcquireTokenSilentMAM(resourceId);

            return token?.Result;
        }
    }
}

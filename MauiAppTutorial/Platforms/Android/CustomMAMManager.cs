using Microsoft.Intune.Mam.Client.App;
using Microsoft.Intune.Mam.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppTutorial
{
    public static class CustomMAMManager
    {
        public static bool iAlreadyEnrolled=false;
        public static void Enroll(string upn,string aaId, string tenantID)
        {
            if (iAlreadyEnrolled == false)
            {
                IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();

                mgr.RegisterAccountForMAM(upn, aaId, tenantID);
                iAlreadyEnrolled = true;
            }
        }
        public static void UnEnroll(string upn, string aaId)
        {
            IMAMEnrollmentManager mgr = MAMComponents.Get<IMAMEnrollmentManager>();
            mgr.UnregisterAccountForMAM(upn,aaId);
            iAlreadyEnrolled = false;
        }
    }
    
}

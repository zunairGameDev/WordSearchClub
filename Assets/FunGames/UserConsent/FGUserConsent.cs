using System;
using FunGames.Core.Modules;
using FunGames.UserConsent.ATT;
using FunGames.UserConsent.GDPR;

namespace FunGames.UserConsent
{
    public class FGUserConsent
    {
        private static Action _onComplete;

        public static event Action OnComplete
        {
            add => _onComplete += value;
            remove => _onComplete -= value;
        }
        
        public static FGModuleCallbacks Callbacks => FGUserConsentManager.Instance.Callbacks;

        public static bool IsAttCompliant => FGATTManager.Instance.isATTCompliant;

        public static FGGDPRStatus GdprStatus => FGGDPRManager.Instance.GdprStatus;

        public static bool IsIABCompliant => FGGDPRManager.Instance.IsIABCompliant;

        public static string ConsentString => FGGDPRManager.Instance.TcfV2String;

        public static bool HasFullConsent
        {
            get => IsAttCompliant && GdprStatus.IsFullyAccepted;
        }

        public static Location Location => FGGDPRManager.Instance.location;

        public static void ShowATT() => FGATTManager.Instance.ShowATT();

        public static void ShowGDPR() => FGGDPRManager.Instance.ShowGDPR();

        internal static void TriggerCompleteCallback()
        {
            _onComplete?.Invoke();
        }
    }
}
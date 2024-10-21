using System;
using System.Collections.Generic;
using FunGames.Analytics;
using FunGames.Core.Modules;
using FunGames.RemoteConfig;
using UnityEngine;

namespace FunGames.UserConsent.GDPR
{
    public class FGGDPRManager : FGModuleAbstract<FGGDPRManager, FGGdprCallbacks, FGGDPRSettings>
    {
        // public override string ModuleName => "GDPR";
        public override FGGDPRSettings Settings => FGGDPRSettings.settings;

        protected override FGModule Parent => FGUserConsentManager.Instance;
        protected override string EventName => "GDPR";

        protected override string RemoteConfigKey => "FGGDPR";

        // public override Color LogColor => FGDebugSettings.settings.TrackingAuthorization;
        public bool IsGDPRAlreadyAnswered => _isGDPRAlreadyAnswered;
        public FGGDPRStatus GdprStatus => _gdprStatus;
        public bool IsIABCompliant => _isIABCompliant;
        public string TcfV2String => _TcfV2String;

        private bool _isGDPRAlreadyAnswered = false;
        private FGGDPRStatus _gdprStatus = FGGDPRStatus.FullyAccepted;
        private bool _isIABCompliant => !String.IsNullOrEmpty(_TcfV2String);
        private string _TcfV2String = String.Empty;

        private const string PP_GDPR_ANSWERED = "isGdrpAnswered";
        private const string PP_GDPR_CONSENT = "hasGdprConsent";

        public const string RC_GDPR_TYPE = "GdprType";
        public const string RC_GDPR_DISPLAY = "GdprDisplay";

        [HideInInspector] public Location location = new Location();

        protected override void InitializeCallbacks()
        {
            InitWithoutTimer();
            FGRemoteConfig.AddDefaultValue(RC_GDPR_TYPE, 0);
            FGRemoteConfig.AddDefaultValue(RC_GDPR_DISPLAY, 0);
            Callbacks.OnInitialized += GDPRInitialized;
            InitializePlayerPrefs();
        }

        protected override void OnAwake()
        {
            // throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new NotImplementedException();
        }

        protected override void InitializeModule()
        {
            InitializationComplete(true);
        }

        protected void GDPRInitialized(bool result)
        {
            SetPlayerPrefsAfterGdpr(FGUserConsent.GdprStatus.IsFullyAccepted);
            Dictionary<string, object> eventData = new Dictionary<string, object>()
            {
                { "TargetedAdvertisingAccepted", _gdprStatus.TargetedAdvertisingAccepted },
                { "AnalyticsAccepted", _gdprStatus.AnalyticsAccepted },
                { "IsAboveAgeLimit", _gdprStatus.IsAboveAgeLimit }
            };
            FGAnalytics.NewDesignEvent("GdprResult", eventData);
            Log("Result : ");
            Log("   - Is Targeted Advertising Accepted ? : " + _gdprStatus.TargetedAdvertisingAccepted);
            Log("   - Is Analytics Accepted ? : " + _gdprStatus.AnalyticsAccepted);
            Log("   - Is Age Above Limit ? : " + _gdprStatus.IsAboveAgeLimit);
        }

        public void ShowGDPR()
        {
            try
            {
                Callbacks?._show?.Invoke();
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Displaying GDPR:" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        private void InitializePlayerPrefs()
        {
            _isGDPRAlreadyAnswered = CheckPlayerPref(PP_GDPR_ANSWERED);
            if (_isGDPRAlreadyAnswered) _gdprStatus.SetGDPRValues(CheckPlayerPref(PP_GDPR_CONSENT));
        }

        private bool CheckPlayerPref(string ppKey)
        {
            return PlayerPrefs.HasKey(ppKey) && PlayerPrefs.GetInt(ppKey).Equals(1);
        }

        private void SetPlayerPrefsAfterGdpr(bool result)
        {
            PlayerPrefs.SetInt(PP_GDPR_ANSWERED, 1);
            PlayerPrefs.SetInt(PP_GDPR_CONSENT, result ? 1 : 0);
        }

        public void ResetPlayerPrefs()
        {
            PlayerPrefs.SetInt(PP_GDPR_ANSWERED, 0);
            PlayerPrefs.SetInt(PP_GDPR_CONSENT, 0);
        }

        protected override void ClearInitialization()
        {
            // FGUserConsent.ATTCallbacks.OnInitialized -= _initialization;
        }

        public void SetTCFString(string tcfV2)
        {
            _TcfV2String = tcfV2;
        }
    }
}
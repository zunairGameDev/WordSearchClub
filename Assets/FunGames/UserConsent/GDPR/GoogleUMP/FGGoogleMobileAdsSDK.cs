using System;
using GoogleMobileAds.Api;

namespace FunGames.UserConsent.GDPR.GoogleUMP
{
    public class FGGoogleMobileAdsSDK : FGSingleton<FGGoogleMobileAdsSDK>
    {
        public bool IsInitialized => _isInitialized;
        public InitializationStatus Status => _initializationStatus;
        
        private bool _isInitialized = false;
        private bool _initializationRequested = false;
        private InitializationStatus _initializationStatus;
        
        
        private Action<InitializationStatus> _onInitializationComplete;

        public event Action<InitializationStatus> OnInitializationComplete
        {
            add => _onInitializationComplete += value;
            remove => _onInitializationComplete -= value;
        }

        public void Initialize()
        {
            if (_initializationRequested) return;
            _initializationRequested = true;

            // When true all events raised by GoogleMobileAds will be raised
            // on the Unity main thread. The default value is false.
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
            
            // MobileAds.GetRequestConfiguration().SameAppKeyEnabled = FGUserConsent.HasFullConsent;
            if (FGGoogleUMPSettings.settings.TestMode &&
                !String.IsNullOrEmpty(FGGoogleUMPSettings.settings.TestDeviceID))
            {
                RequestConfiguration requestConfiguration = new RequestConfiguration();
                requestConfiguration.TestDeviceIds.Add(FGGoogleUMPSettings.settings.TestDeviceID);
                MobileAds.SetRequestConfiguration(requestConfiguration);
            }

            MobileAds.Initialize(OnInitialized);
        }

        private void OnInitialized(InitializationStatus obj)
        {
            _initializationStatus = obj;
            _isInitialized = true;
            _onInitializationComplete?.Invoke(obj);
        }
    }
}
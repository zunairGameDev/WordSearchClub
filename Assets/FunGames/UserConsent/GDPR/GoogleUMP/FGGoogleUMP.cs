using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

namespace FunGames.UserConsent.GDPR.GoogleUMP
{
    public class FGGoogleUMP : FGGdprTcfAbstract<FGGoogleUMP, FGGoogleUMPSettings>
    {
        public override FGGoogleUMPSettings Settings => FGGoogleUMPSettings.settings;
        protected override string EventName => "Google UMP";
        protected override string RemoteConfigKey => "FGGoogleUMP";
        protected override void OnAwake()
        {
            // throw new System.NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new System.NotImplementedException();
        }

        protected override int RemoteConfig => 2;

        private ConsentForm _consentForm;

        protected override void InitSdk()
        {
            if (FGGoogleMobileAdsSDK.Instance.IsInitialized) OnInitialized(FGGoogleMobileAdsSDK.Instance.Status);
            else
            {
                FGGoogleMobileAdsSDK.Instance.OnInitializationComplete += OnInitialized;
                FGGoogleMobileAdsSDK.Instance.Initialize();
            }
        }

        protected override void RequestShow()
        {
            _consentForm.Show(OnShowForm);
        }

        private void OnInitialized(InitializationStatus instanceStatus)
        {
            if (_consentForm != null)
            {
                Show();
                return;
            }

            var debugSettings = new ConsentDebugSettings();
            debugSettings.TestDeviceHashedIds.Add(FGGoogleUMPSettings.settings.TestDeviceID);

            // Set tag for under age of consent.
            // Here false means users are not under age of consent.
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };

            // Check the current consent information status.
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                // Handle the error.
                HandleError(error);
                return;
            }

            Log("Consent info updated with status : " + ConsentInformation.ConsentStatus);

            // If the error is null, the consent information state was updated.
            // You are now ready to check if a form is available.
            if (ConsentInformation.IsConsentFormAvailable())
            {
                LoadConsentForm();
            }
            else
            {
                HandleStatus();
            }
        }

        void LoadConsentForm()
        {
            // Loads a consent form.
            ConsentForm.Load(OnLoadConsentForm);
        }

        void OnLoadConsentForm(ConsentForm consentForm, FormError error)
        {
            if (error != null)
            {
                // Handle the error.
                HandleError(error);
                return;
            }

            Log("Consent loaded with status : " + ConsentInformation.ConsentStatus);

            // The consent form was loaded.
            // Save the consent form for future requests.
            _consentForm = consentForm;

            // You are now ready to show the form.
            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
            {
                Show();
            }

            HandleStatus();
        }

        // void ShowForm()
        // {
        //     _consentForm.Show(OnShowForm);
        // }

        void OnShowForm(FormError error)
        {
            if (error != null)
            {
                // Handle the error.
                HandleError(error);
                return;
            }

            Log("Consent showed with status : " + ConsentInformation.ConsentStatus);

            // Handle dismissal by reloading form.
            LoadConsentForm();
        }

        private void HandleError(FormError error)
        {
            LogError("Error " + error.ErrorCode + " : " + error.Message);
            UpdateConsent(FGGDPRStatus.Refused);
            InitializationComplete(false);
        }

        protected override string GetTcfString()
        {
            return FGCMPReader.GetTCFString();
        }

        private void HandleStatus()
        {
            switch (ConsentInformation.ConsentStatus)
            {
                case ConsentStatus.NotRequired:
                    UpdateConsent(FGGDPRStatus.FullyAccepted);
                    InitializationComplete(true);
                    break;
                case ConsentStatus.Obtained:
                    FGGDPRStatus status = FGUserConsent.GdprStatus;
                    status.TargetedAdvertisingAccepted = FGCMPReader.HasGenericConsent();
                    UpdateConsent(status);
                    InitializationComplete(true);
                    break;
            }
        }
    }
}
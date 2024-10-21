using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;
using FunGames.RemoteConfig;
using UnityEngine;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdAppOpenAbstract<M, MP> : FGMediationAdAbstract<M, MP>
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
        private const string PP_APPOPEN_FREQ = "AppOpenFrequency";
        private const string PP_APPOPEN_CD = "AppOpenCoolDown";

        private int AppOpenCoolDown = 0;
        private int AppOpenFrequency = 0;
        private int _lastAdIteration = 0;
        private bool _hotStart = false;
        private bool _neverDisplayed = true;
        private float _timeWhenAppPaused = 0f;
        private float _timeSpentOutOfApp = 0f;

        public override FGAdType adType => FGAdType.AppOpen;

        protected abstract void InitializeCallbacksImpl();

        protected abstract void ShowAd();

        public override void InitializeCallbacks()
        {
            FGMediation.AddAppOpenCondition(AppOpenFrequencyCondition);
            FGMediation.AddAppOpenCondition(AppOpenFirstSessionCondition);
            FGMediation.AddAppOpenCondition(AppOpenCoolDownCondition);
            FGMediation.AddAppOpenCondition(AppOpenFirstColdStartCondition);

            InitializePlayerPref();

            FGRemoteConfig.Callbacks.OnInitialized += delegate { InitializePlayerPref(); };

            InitializeCallbacksImpl();

            MediationInstance.Callbacks.OnInitialized += delegate { Load(); };
            FGMediation.Callbacks.OnAppOpenAdLoaded += ShowFirstTime;
            _timeWhenAppPaused = Time.realtimeSinceStartup;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus)
            {
                _timeSpentOutOfApp = Time.realtimeSinceStartup - _timeWhenAppPaused;
                Show();
            }
            else
            {
                _hotStart = true;
                _timeWhenAppPaused = Time.realtimeSinceStartup;
            }
        }

        protected override void ShowImpl()
        {
            bool condition = FGMediationManager.Instance.MatchAppOpenCondition();
            MediationInstance.Log("Condition for AppOpen: " + condition);
            if (!condition) return;
            ShowAd();
        }

        protected override void HideImpl()
        {
            // Do nothing
        }

        private void ShowFirstTime(FGAdInfo adInfo)
        {
            Show();
            FGMediation.Callbacks.OnAppOpenAdLoaded -= ShowFirstTime;
        }

        protected override void TriggerLoadedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Loaded, AdType.AppOpen, LoadedAdInfo.NetworkName, LoadedAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdLoaded?.Invoke(LoadedAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdLoaded?.Invoke(LoadedAdInfo);
        }

        protected override void TriggerDisplayedEventImpl()
        {
            _neverDisplayed = false;
            FGAnalytics.NewAdEvent(AdAction.Show, AdType.AppOpen, ShowingAdInfo.NetworkName, ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdDisplayed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdDisplayed?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(true);
        }

        protected override void TriggerClosedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Dismissed, AdType.AppOpen, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdDismissed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdDismissed?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerClickedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Clicked, AdType.AppOpen, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdClicked?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdClicked?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerImpressionEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Impression, AdType.AppOpen, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
        }

        protected override void TriggerLoadFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.AppOpen, "Max",
                FGMediationManager.FAILED_LOAD_PLACEMENT_NAME);
            MediationInstance.Callbacks._OnAppOpenAdFailedToLoad?.Invoke();
            FGMediationManager.Instance.Callbacks._OnAppOpenAdFailedToLoad?.Invoke();
        }

        protected override void TriggerDisplayFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.AppOpen, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnAppOpenAdFailedToDisplay?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnAppOpenAdFailedToDisplay?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(false);
        }

        private void InitializePlayerPref()
        {
            AppOpenFrequency = FGRemoteConfig.GetIntValue(FGMediationManager.RC_APPOPEN_FREQ);
            AppOpenCoolDown = FGRemoteConfig.GetIntValue(FGMediationManager.RC_APPOPEN_CD);
            PlayerPrefs.SetInt(PP_APPOPEN_FREQ, AppOpenFrequency);
            PlayerPrefs.SetInt(PP_APPOPEN_CD, AppOpenCoolDown);
            _lastAdIteration = 0;
        }

        private bool AppOpenFrequencyCondition()
        {
            bool result = false;

            if (AppOpenFrequency != 0 && _neverDisplayed)
            {
                result = true;
            }
            else if (AppOpenFrequency != 0 && _lastAdIteration == AppOpenFrequency - 1)
            {
                _lastAdIteration = 0;
                result = true;
            }
            else
            {
                _lastAdIteration++;
            }

            // MediationInstance.Log("AppOpenFrequencyCondition: " + result);
            return result;
        }

        private bool AppOpenFirstSessionCondition()
        {
            int firstSessionCondition = FGRemoteConfig.GetIntValue(FGMediationManager.RC_APPOPEN_FIRST_SESSION);
            bool result = FunGamesSDK.CurrentSessionNumber >= firstSessionCondition;
            // MediationInstance.Log("AppOpenFirstSessionCondition: " + result);
            return result;
        }

        private bool AppOpenFirstColdStartCondition()
        {
            bool isFirstSession = 1f.Equals(FunGamesSDK.CurrentSessionNumber);
            bool isHotStart = _hotStart;
            bool activateOnColdStart = FGRemoteConfig.GetBooleanValue(FGMediationManager.RC_APPOPEN_COLD_START);
            bool result = isFirstSession ? isHotStart : activateOnColdStart;
            // MediationInstance.Log("AppOpenFirstColdStartCondition: " + result);
            return result;
        }

        private bool AppOpenCoolDownCondition()
        {
            float timeSinceLastInter = FGMediation.TimeSinceLastInterstitial + _timeSpentOutOfApp;
            bool result = timeSinceLastInter >= AppOpenCoolDown || FGMediation.TimeSinceLastInterstitial < 0;
            // MediationInstance.Log("AppOpenCoolDownCondition: " + result + "\n" +
            //                       "TimeSinceLastInterstitial: " + timeSinceLastInter);
            return result;
        }
    }
}
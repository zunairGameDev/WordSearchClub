using System;
using System.Collections.Generic;
using FunGames.Core;
using FunGames.Core.Modules;
using FunGames.RemoteConfig;
using UnityEngine;

namespace FunGames.Mediation
{
    public class FGMediationManager : FGModuleAbstract<FGMediationManager, FGMediationCallbacks, FGMediationSettings>
    {
        public override FGMediationSettings Settings => FGMediationSettings.settings;
        protected override FGModule Parent => FGCore.Instance;
        protected override string EventName => "Mediation";
        protected override string RemoteConfigKey => "FGMediation";

        private float _timeSinceLastInterstitial = -1;

        public const string RC_APPOPEN_FREQ = "AppOpenFrequency";
        public const string RC_APPOPEN_CD = "AppOpenCoolDown";
        public const string RC_APPOPEN_FIRST_SESSION = "AppOpenFirstSession";
        public const string RC_APPOPEN_COLD_START = "AppOpenColdStart";

        public const string DEFAULT_PLACEMENT_NAME = "default";
        public const string FAILED_LOAD_PLACEMENT_NAME = "fail_load";

        private List<Func<bool>> _appOpenConditions = new List<Func<bool>>();

        public List<IFGMediationAd> AllAdUnits =
            new List<IFGMediationAd>();

        [HideInInspector] public int CurrentLoadingAdIndex = 0;

        public bool IsBannerReady => IsAdReady(FGAdType.Banner);
        public bool IsInterstitialReady => IsAdReady(FGAdType.Interstitial);
        public bool IsRewardedReady => IsAdReady(FGAdType.Rewarded);
        public bool IsMrecReady => IsAdReady(FGAdType.MREC);
        public bool IsAppOpenReady => IsAdReady(FGAdType.AppOpen);

        public float TimeSinceLastInterstitial => _timeSinceLastInterstitial;

        [HideInInspector] public bool IsShowBannerRequested = false;

        protected override void InitializeCallbacks()
        {
            FGRemoteConfig.AddDefaultValue(RC_APPOPEN_FREQ, 0);
            FGRemoteConfig.AddDefaultValue(RC_APPOPEN_CD, 10);
            FGRemoteConfig.AddDefaultValue(RC_APPOPEN_FIRST_SESSION, 1);
            FGRemoteConfig.AddDefaultValue(RC_APPOPEN_COLD_START, 1);

            Callbacks.OnInterstitialAdDisplayed += adInfo => _timeSinceLastInterstitial = 0f;
            Callbacks.OnAppOpenDisplayed += adInfo => _timeSinceLastInterstitial = 0f;

            FunGamesSDK.Callbacks.OnAdsRemoved += FGMediation.HideBanner;
        }

        protected override void OnAwake()
        {
            FunGamesSDK.Callbacks.Initialization += Initialize;
        }

        protected override void OnStart()
        {
         //   
        }

        protected override void InitializeModule()
        {
            InitializationComplete(true);
        }

        private void Update()
        {
            if (_timeSinceLastInterstitial >= 0f) _timeSinceLastInterstitial += Time.deltaTime;
        }

        public void ShowBanner(string placementName)
        {
            try
            {
                Callbacks._ShowBanner?.Invoke(placementName);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Banner Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowBanner(string placementName, string unitId)
        {
            try
            {
                Callbacks._ShowSpecificBanner?.Invoke(placementName, unitId);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Banner Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void HideBanner()
        {
            try
            {
                Callbacks._HideBanner?.Invoke();
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Closing Banner Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowInterstitial(Action<bool> endOfAdCallback, string placementName)
        {
            try
            {
                Callbacks._ShowInter?.Invoke(endOfAdCallback, placementName);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Interstitial Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowInterstitial(Action<bool> endOfAdCallback, string placementName, string unitId)
        {
            try
            {
                Callbacks._ShowSpecificInter?.Invoke(endOfAdCallback, placementName, unitId);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Interstitial Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowRewarded(Action<bool> rewardedCallback, string placementName)
        {
            try
            {
                Callbacks._ShowRV?.Invoke(rewardedCallback, placementName);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Rewarded Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowRewarded(Action<bool> rewardedCallback, string placementName, string unitId)
        {
            try
            {
                Callbacks._ShowSpecificRV?.Invoke(rewardedCallback, placementName, unitId);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing Rewarded Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowMrec(Action<bool> endOfAdCallback, string placementName)
        {
            try
            {
                Callbacks._ShowMrec?.Invoke(endOfAdCallback, placementName);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing MREC Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowMrec(Action<bool> endOfAdCallback, string placementName, string unitId)
        {
            try
            {
                Callbacks._ShowSpecificMrec?.Invoke(endOfAdCallback, placementName, unitId);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing MREC Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void HideMrec()
        {
            try
            {
                Callbacks._HideMrec?.Invoke();
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Closing MREC Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowAppOpen(Action<bool> endOfAdCallback, string placementName)
        {
            try
            {
                Callbacks._ShowAppOpen?.Invoke(endOfAdCallback, placementName);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing AppOpen Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void ShowAppOpen(Action<bool> endOfAdCallback, string placementName, string unitId)
        {
            try
            {
                Callbacks._ShowSpecificAppOpen?.Invoke(endOfAdCallback, placementName, unitId);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Showing AppOpen Ad :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void AddAppOpenCondition(Func<bool> condition)
        {
            _appOpenConditions?.Add(condition);
        }

        public bool MatchAppOpenCondition()
        {
            return CalculateCondition(_appOpenConditions?.ToArray());
        }

        public bool IsAdReady(FGAdType adType)
        {
            IFGMediationAd mediationAd = AllAdUnits.Find(ad => isAdReady(ad, adType));
            return mediationAd != null;
        }

        public bool IsAdShowing(FGAdType adType)
        {
            IFGMediationAd mediationAd = AllAdUnits.Find(ad => isAdShowing(ad, adType));
            return mediationAd != null;
        }

        private bool isAdReady(IFGMediationAd mediationAd, FGAdType adType)
        {
            return adType.Equals(mediationAd.adType) && mediationAd.IsReady();
        }

        private bool isAdShowing(IFGMediationAd mediationAd, FGAdType adType)
        {
            return adType.Equals(mediationAd.adType) && mediationAd.IsShowing();
        }

        private bool CalculateCondition(Func<bool>[] conditions)
        {
            bool result = true;
            if (conditions.Length == 0) return false;
            foreach (var condition in conditions)
            {
                result &= condition.Invoke();
            }
            return result;
        }

        protected override void ClearInitialization()
        {
            // throw new NotImplementedException();
        }
    }
}
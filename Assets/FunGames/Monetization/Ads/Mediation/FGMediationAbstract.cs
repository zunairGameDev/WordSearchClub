using System;
using System.Collections.Generic;
using FunGames.Core;
using FunGames.UserConsent;
using FunGames.Core.Modules;
using FunGames.Core.Settings;

namespace FunGames.Mediation
{
    public abstract class FGMediationAbstract<M, S> : FGModuleAbstract<M, FGMediationCallbacks, S>
        where M : FGModuleAbstract<M, FGMediationCallbacks, S>
        where S : IFGModuleSettings
    {
        protected override FGModule Parent => FGMediationManager.Instance;

        internal FGAdInfo _loadedBannerAd;

        // private Action _adInitialization;

        public Dictionary<string, IFGMediationAd> FGMediationAds =
            new Dictionary<string, IFGMediationAd>();

        private int _loadingAdIndex = 0;

        protected abstract void LoadAds();
        protected abstract void ShowBannerAd(string placementName);
        protected abstract void ShowBannerAd(string placementName, string unitId);
        protected abstract void HideBannerAd();

        protected abstract void ShowInterstitialAd(Action<bool> callback, string placementName);

        protected abstract void ShowInterstitialAd(Action<bool> callback, string placementName, string unitId);

        protected abstract void ShowRewardedAd(Action<bool> callback, string placementName);

        protected abstract void ShowRewardedAd(Action<bool> callback, string placementName, string unitId);

        protected abstract void ShowAppOpen(Action<bool> callback, string placementName);
        protected abstract void ShowAppOpen(Action<bool> callback, string placementName, string unitId);

        protected abstract void ShowMrecAd(Action<bool> callback, string placementName);
        protected abstract void ShowMrecAd(Action<bool> callback, string placementName, string unitId);
        protected abstract void HideMrecAd();

        protected override void InitializeCallbacks()
        {
            // _adInitialization = ;
            Callbacks.InitializeAds += LoadAds;

            if (FGUserConsentManager.Instance.IsInitialized) Callbacks.OnInitialized += (b)=> InitializeAds();
            else FGUserConsent.OnComplete += InitializeAds;
            FGMediationManager.Instance.Callbacks.Initialization += Initialize;
            FGMediationManager.Instance.Callbacks.InitializeAds += LoadAds;
            FGMediationManager.Instance.Callbacks.ShowBanner += ShowBannerAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificBanner += ShowBannerAd;
            FGMediationManager.Instance.Callbacks.HideBanner += HideBannerAd;
            FGMediationManager.Instance.Callbacks.ShowInterstitial += ShowInterstitialAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificInterstitial += ShowInterstitialAd;
            FGMediationManager.Instance.Callbacks.ShowRewarded += ShowRewardedAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificRewarded += ShowRewardedAd;
            FGMediationManager.Instance.Callbacks.ShowAppOpen += ShowAppOpen;
            FGMediationManager.Instance.Callbacks.ShowSpecificAppOpen += ShowAppOpen;
            FGMediationManager.Instance.Callbacks.ShowMrec += ShowMrecAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificMrec += ShowMrecAd;
            FGMediationManager.Instance.Callbacks.HideMrec += HideMrecAd;
        }

        protected void InitializeAds()
        {
            if (!IsInitialized)
            {
                Callbacks.OnInitialized += delegate { InitializeAds(); };
                return;
            }

            Callbacks._InitializeAds?.Invoke();
        }

        public void ShowAd(FGAdType adType, string adUnit, string placementName, Action<bool> callback)
        {
            IFGMediationAd mediationAd =
                FGMediationManager.Instance.AllAdUnits.Find(ad => isRequestedAd(ad, adType, adUnit));
            if (mediationAd == null || !mediationAd.IsReady())
            {
                IFGMediationAd other =
                    FGMediationManager.Instance.AllAdUnits.Find(ad => isAvailableAdOfSameType(ad, adType));
                if (other != null) mediationAd = other;
            }

            if (mediationAd != null) mediationAd.Show(placementName, callback);
        }

        private static bool isRequestedAd(IFGMediationAd mediationAd, FGAdType adType, string adUnit)
        {
            return adType.Equals(mediationAd.adType) && adUnit.Equals(mediationAd.AdUnitId);
        }

        private static bool isAvailableAdOfSameType(IFGMediationAd mediationAd, FGAdType adType)
        {
            return adType.Equals(mediationAd.adType) && mediationAd.IsReady();
        }

        protected void CrosspromoImpressionValidated(string arg1, FGAdInfo arg2)
        {
            Log("Crosspromo Impression validated");
            Callbacks._OnCrosspromoAdImpression?.Invoke(arg1, arg2);
            FGMediationManager.Instance.Callbacks._OnCrosspromoAdImpression?.Invoke(arg1, arg2);
        }

        protected override void ClearInitialization()
        {
            FGUserConsent.OnComplete -= InitializeAds;
            FGMediationManager.Instance.Callbacks.Initialization -= Initialize;
            FGMediationManager.Instance.Callbacks.InitializeAds -= LoadAds;
            FGMediationManager.Instance.Callbacks.ShowBanner -= ShowBannerAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificBanner -= ShowBannerAd;
            FGMediationManager.Instance.Callbacks.HideBanner -= HideBannerAd;
            FGMediationManager.Instance.Callbacks.ShowInterstitial -= ShowInterstitialAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificInterstitial -= ShowInterstitialAd;
            FGMediationManager.Instance.Callbacks.ShowRewarded -= ShowRewardedAd;
            FGMediationManager.Instance.Callbacks.ShowSpecificRewarded -= ShowRewardedAd;
            FGMediationManager.Instance.Callbacks.ShowAppOpen -= ShowAppOpen;
        }
    }
}
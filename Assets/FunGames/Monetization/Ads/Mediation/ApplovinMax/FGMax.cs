using System;
using System.Collections.Generic;
using FunGames.Core;
using FunGames.UserConsent;
using FunGames.Core.Settings;
using UnityEngine;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMax : FGMediationAbstract<FGMax, IFGModuleSettings>
    {
        internal FGMediationMaxInterstitialAd DefaultInterstitial;
        internal FGMediationMaxRewardedAd DefaultRewarded;
        internal FGMediationMaxBannerAd DefaultBanner;
        internal FGMediationMaxAppOpenAd DefaultAppOpen;
        internal FGMediationMaxMrecAd DefaultMrec;

        public override IFGModuleSettings Settings => FGApplovinMaxSettings.settings;
        protected override string EventName => "ApplovinMax";
        protected override string RemoteConfigKey => "FGApplovinMax";

        public const string MAX_EVENT_LOADING_TIME = "MaxAdLoadingTime";
        private FGMediationMaxBannerAd currentBannerAd;
        private Action _updateConsentCallback;

        protected override void InitializeCallbacks()
        {
            base.InitializeCallbacks();
            _updateConsentCallback = UpdateConsent;
            FGUserConsent.OnComplete += _updateConsentCallback;

            FGMaxLoadEventSender.Instance.Initialize();
        }

        protected override void OnAwake()
        {
            // throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new NotImplementedException();
        }

        // Start is called on the start of FunGamesAds
        protected override void InitializeModule()
        {
            InitializeMaxAds();
            MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
            {
                InitializationComplete(MaxSdk.IsInitialized());
            };

            UpdateConsent();
            MaxSdk.SetSdkKey(FGApplovinMaxSettings.settings.maxSdkKey);
            MaxSdk.InitializeSdk();
            LogConfig();
        }

        private void UpdateConsent()
        {
            Log("Consent updated : " + FGUserConsent.HasFullConsent);
            MaxSdk.SetHasUserConsent(FGUserConsent.HasFullConsent);
            MaxSdk.SetDoNotSell(!FGUserConsent.HasFullConsent);
            if (FGUserConsent.HasFullConsent) MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        }

        private void InitializeMaxAds()
        {
            DefaultBanner = FGMediationMaxBannerAd.Initialize(FGAdType.Banner,
                FGApplovinMaxSettings.settings.DefaultBannerId.Trim());
            DefaultInterstitial = FGMediationMaxInterstitialAd.Initialize(FGAdType.Interstitial,
                FGApplovinMaxSettings.settings.DefaultInterstitialId.Trim());
            DefaultRewarded = FGMediationMaxRewardedAd.Initialize(FGAdType.Rewarded,
                FGApplovinMaxSettings.settings.DefaultRewardedId.Trim());
            DefaultAppOpen = FGMediationMaxAppOpenAd.Initialize(FGAdType.AppOpen,
                FGApplovinMaxSettings.settings.DefaultAppOpenId.Trim());
            DefaultMrec =
                FGMediationMaxMrecAd.Initialize(FGAdType.MREC, FGApplovinMaxSettings.settings.DefaultMrecId.Trim());


            if (DefaultInterstitial) FGMediationAds.Add(DefaultInterstitial.AdUnitId, DefaultInterstitial);
            if (DefaultRewarded) FGMediationAds.Add(DefaultRewarded.AdUnitId, DefaultRewarded);
            if (DefaultBanner) FGMediationAds.Add(DefaultBanner.AdUnitId, DefaultBanner);
            if (DefaultAppOpen) FGMediationAds.Add(DefaultAppOpen.AdUnitId, DefaultAppOpen);
            if (DefaultMrec) FGMediationAds.Add(DefaultMrec.AdUnitId, DefaultMrec);

            foreach (var fgAdUnit in FGApplovinMaxSettings.settings.AdditionalUnitIds)
            {
                if (isForCurrentPlatform(fgAdUnit)) FGMediationAds.Add(fgAdUnit.unitId.Trim(), InitializeAd(fgAdUnit));
            }
        }

        private IFGMediationAd InitializeAd(FGAdUnit fgAdUnit)
        {
            switch (fgAdUnit.adType)
            {
                case FGAdType.Interstitial:
                    return FGMediationMaxInterstitialAd.Initialize(FGAdType.Interstitial, fgAdUnit.unitId.Trim());
                case FGAdType.Rewarded:
                    return FGMediationMaxRewardedAd.Initialize(FGAdType.Rewarded, fgAdUnit.unitId.Trim());
                case FGAdType.Banner:
                    return FGMediationMaxBannerAd.Initialize(FGAdType.Banner, fgAdUnit.unitId.Trim());
                case FGAdType.MREC:
                    return FGMediationMaxMrecAd.Initialize(FGAdType.MREC, fgAdUnit.unitId.Trim());
            }

            return null;
        }

        private bool isForCurrentPlatform(FGAdUnit fgAdUnit)
        {
            if (String.IsNullOrEmpty(fgAdUnit.unitId)) return false;

            if (fgAdUnit.platform.Equals(FGPlatform.Android))
            {
#if UNITY_ANDROID
                return true;
#endif
            }

            if (fgAdUnit.platform.Equals(FGPlatform.IOS))
            {
#if UNITY_IOS
                return true;
#endif
            }

            return false;
        }

        private void LogConfig()
        {
            try
            {
                Dictionary<string, Dictionary<string, object>>
                    config = new Dictionary<string, Dictionary<string, object>>();
                config.Add("ApplovinMaxSDK", new Dictionary<string, object> { { "Version", MaxSdk.Version } });

                foreach (var network in MaxSdk.GetAvailableMediatedNetworks())
                {
                    if (config.ContainsKey(network.Name)) continue;
                    config.Add(network.Name,
                        new Dictionary<string, object>
                            { { "Adapter_Version", network.AdapterVersion }, { "Sdk_Version", network.SdkVersion } });
                }

                foreach (var info in config)
                {
                    // FGAnalytics.NewDesignEvent("MaxConfig:" + info.Key, info.Value);
                    Log("Network: " + info.Key);
                    foreach (var versions in info.Value)
                    {
                        Log("\t" + versions.Key + " : " + versions.Value);
                    }
                }
            }
            catch (Exception e)
            {
                LogError("Failed to Log config: " + e.Message);
            }
        }

        protected override void LoadAds()
        {
            Log("Initialize Ads");
            foreach (var fgMediationAd in FGMediationAds)
            {
                fgMediationAd.Value.Load();
            }
        }

        protected override void ShowBannerAd(string placementName)
        {
            if (DefaultBanner == null) return;
            ShowAd(FGAdType.Banner, DefaultBanner.AdUnitId, placementName, null);
        }

        protected override void ShowBannerAd(string placementName, string unitId)
        {
            ShowAd(FGAdType.Banner, unitId, placementName, null);
        }

        protected override void HideBannerAd()
        {
            foreach (var fgMediationAd in FGMediationAds)
            {
                if (fgMediationAd.Value is FGMediationMaxBannerAd) ((FGMediationMaxBannerAd)fgMediationAd.Value).Hide();
            }
        }

        protected override void ShowInterstitialAd(Action<bool> callback, string placementName)
        {
            if (DefaultInterstitial == null) return;
            ShowAd(FGAdType.Interstitial, DefaultInterstitial.AdUnitId, placementName, callback);
        }

        protected override void ShowInterstitialAd(Action<bool> callback, string placementName, string unitId)
        {
            ShowAd(FGAdType.Interstitial, unitId, placementName, callback);
        }

        protected override void ShowRewardedAd(Action<bool> callback, string placementName)
        {
            if (DefaultRewarded == null) return;
            ShowAd(FGAdType.Rewarded, DefaultRewarded.AdUnitId, placementName, callback);
        }

        protected override void ShowRewardedAd(Action<bool> callback, string placementName, string unitId)
        {
            ShowAd(FGAdType.Rewarded, unitId, placementName, callback);
        }

        protected override void ShowAppOpen(Action<bool> callback, string placementName)
        {
            if (DefaultAppOpen == null) return;
            ShowAd(FGAdType.Rewarded, DefaultAppOpen.AdUnitId, placementName, callback);
        }

        protected override void ShowAppOpen(Action<bool> callback, string placementName, string unitId)
        {
            ShowAd(FGAdType.Rewarded, unitId, placementName, callback);
        }

        protected override void ShowMrecAd(Action<bool> callback, string placementName)
        {
            if (DefaultMrec == null) return;
            ShowAd(FGAdType.MREC, DefaultMrec.AdUnitId, placementName, callback);
        }

        protected override void ShowMrecAd(Action<bool> callback, string placementName, string unitId)
        {
            ShowAd(FGAdType.MREC, unitId, placementName, callback);
        }

        protected override void HideMrecAd()
        {
            foreach (var fgMediationAd in FGMediationAds)
            {
                if (fgMediationAd.Value is FGMediationMaxMrecAd) ((FGMediationMaxMrecAd)fgMediationAd.Value).Hide();
            }
        }

        internal FGAdInfo FGAdInfo(MaxSdkBase.AdInfo adinfo)
        {
            FGAdInfo fgAdInfo = new FGAdInfo(adinfo.AdUnitIdentifier, adinfo.AdFormat, adinfo.NetworkName,
                adinfo.NetworkPlacement, adinfo.Placement, adinfo.CreativeIdentifier, adinfo.Revenue,
                adinfo.RevenuePrecision);
            return fgAdInfo;
        }

        internal void LoadInterstitials()
        {
            foreach (IFGMediationAd fgMediationAd in FGMediationAds.Values)
            {
                if (fgMediationAd is FGMediationMaxInterstitialAd) fgMediationAd.Load();
            }
        }

        internal void LoadRewarded()
        {
            foreach (IFGMediationAd fgMediationAd in FGMediationAds.Values)
            {
                if (fgMediationAd is FGMediationMaxRewardedAd) fgMediationAd.Load();
            }
        }

        internal void LoadBanners()
        {
            foreach (IFGMediationAd fgMediationAd in FGMediationAds.Values)
            {
                if (fgMediationAd is FGMediationMaxBannerAd) fgMediationAd.Load();
            }
        }

        internal void LoadMrecs()
        {
            foreach (IFGMediationAd fgMediationAd in FGMediationAds.Values)
            {
                if (fgMediationAd is FGMediationMaxMrecAd) fgMediationAd.Load();
            }
        }

        protected override void ClearInitialization()
        {
            base.ClearInitialization();
            FGUserConsent.OnComplete -= _updateConsentCallback;
        }
    }
}
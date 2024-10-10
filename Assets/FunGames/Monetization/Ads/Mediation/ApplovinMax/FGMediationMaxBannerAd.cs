using FunGames.Core.Settings;
using UnityEngine;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMediationMaxBannerAd : FGMediationAdBannerAbstract<FGMediationMaxBannerAd, FGMax>
    {
        protected override FGMediationAbstract<FGMax, IFGModuleSettings> MediationInstance => FGMax.Instance;

        private bool _isCreated = false;

        public override void InitializeCallbacks()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoaded;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdImpressionEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedToLoadEvent;
        }

        protected override void ShowAd()
        {
            MaxSdk.SetBannerPlacement(AdUnitId, ShowingAdInfo.Placement);
            MaxSdk.ShowBanner(AdUnitId);
        }

        protected override void HideAd()
        {
            MaxSdk.HideBanner(AdUnitId);
        }

        protected override void LoadAd()
        {
            if (!_isCreated)
            {
                MaxSdk.CreateBanner(AdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
                MaxSdk.SetBannerBackgroundColor(AdUnitId, FGApplovinMaxSettings.settings.BannerBackgroundColor);
                _isCreated = true;
                return;
            }
            
            MaxSdk.StopBannerAutoRefresh(AdUnitId);
            MaxSdk.LoadBanner(AdUnitId);
            MaxSdk.StartBannerAutoRefresh(AdUnitId);
        }


        private void OnBannerAdLoaded(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadedEvent(FGMax.Instance.FGAdInfo(adInfo));
            SendLoadingTimeEvent(FGMax.MAX_EVENT_LOADING_TIME,adInfo.LatencyMillis);
        }

        private void OnBannerAdImpressionEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerImpressionEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnBannerAdFailedToLoadEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadFailedEvent();
            FGMax.Instance.LogWarning(errorInfo.Message);
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClickedEvent();
        }
    }
}
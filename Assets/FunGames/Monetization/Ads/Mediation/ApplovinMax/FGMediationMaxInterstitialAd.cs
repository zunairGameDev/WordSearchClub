using FunGames.Core.Settings;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMediationMaxInterstitialAd : FGMediationAdInterstitialAbstract<FGMediationMaxInterstitialAd, FGMax>
    {
        protected override FGMediationAbstract<FGMax, IFGModuleSettings> MediationInstance => FGMax.Instance;

        public override void InitializeCallbacks()
        {
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterAdImpressionEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedToLoadEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialFailedToDisplayEvent;
        }

        protected override void LoadImpl()
        {
            MaxSdk.LoadInterstitial(AdUnitId);
        }

        public override bool IsReady()
        {
            return MaxSdk.IsInterstitialReady(AdUnitId);
        }

        protected override void ShowAd(string placementName)
        {
            MaxSdk.ShowInterstitial(AdUnitId, placementName);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadedEvent(FGMax.Instance.FGAdInfo(adInfo));
            SendLoadingTimeEvent(FGMax.MAX_EVENT_LOADING_TIME,adInfo.LatencyMillis);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayedEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClosedEvent();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClickedEvent();
        }

        private void OnInterAdImpressionEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId))
            {
                FGMax.Instance.Log("Not the same ids : " + adUnitId + " - " + AdUnitId);
                return;
            }

            TriggerImpressionEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnInterstitialFailedToLoadEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadFailedEvent();
        }

        private void OnInterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayFailedEvent();
        }
    }
}
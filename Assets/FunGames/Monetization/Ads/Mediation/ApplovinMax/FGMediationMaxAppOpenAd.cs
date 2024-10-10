using FunGames.Core;
using FunGames.Core.Settings;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMediationMaxAppOpenAd : FGMediationAdAppOpenAbstract<FGMediationMaxAppOpenAd, FGMax>
    {
        protected override FGMediationAbstract<FGMax, IFGModuleSettings> MediationInstance => FGMax.Instance;

        protected override void InitializeCallbacksImpl()
        {
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
            MaxSdkCallbacks.AppOpen.OnAdClickedEvent += OnAppOpenClickedEvent;
            MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenAdImpressionEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenFailedToLoadEvent;
            MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += OnAppOpenFailedToDisplayEvent;
            MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        }

        protected override void ShowAd()
        {
            MaxSdk.ShowAppOpenAd(AdUnitId);
        }
        
        protected override void LoadImpl()
        {
            MaxSdk.LoadAppOpenAd(AdUnitId);
        }

        public override bool IsReady()
        {
            if (FunGamesSDK.IsNoAd(FGAdType.AppOpen)) return false;
            return MaxSdk.IsAppOpenAdReady(AdUnitId);
        }

        private void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadedEvent(FGMax.Instance.FGAdInfo(adInfo));
            SendLoadingTimeEvent(FGMax.MAX_EVENT_LOADING_TIME,adInfo.LatencyMillis);
        }

        private void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayedEvent(FGMax.Instance.FGAdInfo(adInfo) );
        }

        public void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClosedEvent();
        }

        private void OnAppOpenClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClickedEvent();
        }

        private void OnAppOpenAdImpressionEvent(string adType, MaxSdkBase.AdInfo adInfo)
        {
            if (!adInfo.AdUnitIdentifier.Equals(AdUnitId)) return;
            TriggerImpressionEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnAppOpenFailedToLoadEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadFailedEvent();
        }

        private void OnAppOpenFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayFailedEvent();
            FGMax.Instance.Log(errorInfo.ToString());
        }
    }
}
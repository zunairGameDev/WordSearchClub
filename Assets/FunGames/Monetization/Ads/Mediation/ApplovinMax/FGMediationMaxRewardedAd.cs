using FunGames.Core.Settings;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMediationMaxRewardedAd : FGMediationAdRewardedAbstract<FGMediationMaxRewardedAd, FGMax>
    {
        protected override FGMediationAbstract<FGMax, IFGModuleSettings> MediationInstance => FGMax.Instance;

        public override void InitializeCallbacks()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdImpressionEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedToLoadEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        }

        protected override void LoadImpl()
        {
            MaxSdk.LoadRewardedAd(AdUnitId);
        }

        public override bool IsReady()
        {
            return MaxSdk.IsRewardedAdReady(AdUnitId);
        }

        protected override void ShowAd()
        {
            MaxSdk.ShowRewardedAd(AdUnitId, ShowingAdInfo.Placement);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadedEvent(FGMax.Instance.FGAdInfo(adInfo));
            SendLoadingTimeEvent(FGMax.MAX_EVENT_LOADING_TIME,adInfo.LatencyMillis);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayedEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClosedEvent();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClickedEvent();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerRewardEvent();
        }


        private void OnRewardedAdFailedToLoadEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadFailedEvent();
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayFailedEvent();
        }

        private void OnRewardedAdImpressionEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerImpressionEvent(FGMax.Instance.FGAdInfo(adInfo));
        }
    }
}
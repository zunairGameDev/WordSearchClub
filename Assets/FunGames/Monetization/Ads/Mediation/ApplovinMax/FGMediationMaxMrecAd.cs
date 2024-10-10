using FunGames.Core.Settings;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMediationMaxMrecAd : FGMediationAdMrecAbstract<FGMediationMaxMrecAd, FGMax>
    {
        protected override FGMediationAbstract<FGMax, IFGModuleSettings> MediationInstance => FGMax.Instance;

        public override void InitializeCallbacks()
        {
            MaxSdkCallbacks.MRec.OnAdLoadedEvent      += OnMRecAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent  += OnMRecAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdClickedEvent     += OnMRecAdClickedEvent;
            MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
            // MaxSdkCallbacks.MRec.OnAdExpandedEvent    += OnMRecAdExpandedEvent;
            // MaxSdkCallbacks.MRec.OnAdCollapsedEvent   += OnMRecAdCollapsedEvent;
        }

        protected override void LoadImpl()
        {
            MaxSdk.CreateMRec(AdUnitId, MaxSdkBase.AdViewPosition.Centered);
        }

        protected override void ShowAd()
        {
            MaxSdk.SetMRecPlacement(AdUnitId, ShowingAdInfo.Placement);
            MaxSdk.ShowMRec(AdUnitId);
        }

        protected override void HideAd()
        {
            MaxSdk.HideMRec(AdUnitId);
        }

        public override void UpdateAutoRefresh()
        {
            MaxSdk.StopMRecAutoRefresh(AdUnitId);
            MaxSdk.LoadMRec(AdUnitId);
            MaxSdk.StartMRecAutoRefresh(AdUnitId);
        }
        
        private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadedEvent(FGMax.Instance.FGAdInfo(adInfo));
            SendLoadingTimeEvent(FGMax.MAX_EVENT_LOADING_TIME,adInfo.LatencyMillis);
        }

        private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClickedEvent();
        }

        private void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerImpressionEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerDisplayedEvent(FGMax.Instance.FGAdInfo(adInfo));
        }

        private void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerClosedEvent();
        }
        
        private void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo adInfo)
        {
            if (!adUnitId.Equals(AdUnitId)) return;
            TriggerLoadFailedEvent();
        }
    }
}
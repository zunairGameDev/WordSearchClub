using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdMrecAbstract<M, MP> : FGMediationAdAbstract<M, MP>
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
        private bool _isMrecLoaded;
        private bool _showMrecAsked;
        private bool _isMrecShowing;

        public override FGAdType adType => FGAdType.MREC;

        protected abstract void ShowAd();
        protected abstract void HideAd();

        public abstract void UpdateAutoRefresh();

        public override bool IsReady()
        {
            if (FunGamesSDK.IsNoAd(adType)) return false;
            return _isMrecLoaded;
        }

        protected override void ShowImpl()
        {
            if (_isMrecShowing || _showMrecAsked) return;

            ShowAd();
            _isMrecShowing = _isMrecLoaded;
            _showMrecAsked = true;

            if (_isMrecLoaded) TriggerDisplayedEvent(ShowingAdInfo);
        }

        protected override void HideImpl()
        {
            if (_isMrecShowing) TriggerClosedEvent();

            HideAd();
            _isMrecShowing = false;
            _showMrecAsked = false;
        }

        protected override void TriggerLoadedEventImpl()
        {
            _isMrecLoaded = true;
            MediationInstance.Log("Mrec Ad Loaded");
            FGAnalytics.NewAdEvent(AdAction.Loaded, AdType.MREC, LoadedAdInfo.NetworkName,
                LoadedAdInfo.Placement);
            MediationInstance.Callbacks._OnMrecAdLoaded?.Invoke(LoadedAdInfo);
            FGMediationManager.Instance.Callbacks._OnMrecAdLoaded?.Invoke(LoadedAdInfo);
        }

        protected override void TriggerDisplayedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Show, AdType.MREC, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnMrecAdDisplayed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnMrecAdDisplayed?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(true);
        }

        protected override void TriggerClosedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Dismissed, AdType.MREC, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnMrecAdClosed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnMrecAdClosed?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerClickedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Clicked, AdType.MREC, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnMrecAdClicked?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnMrecAdClicked?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerImpressionEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Impression, AdType.MREC, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnMrecAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnMrecAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
        }

        protected override void TriggerLoadFailedEventImpl()
        {
            _isMrecLoaded = false;
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.MREC, "Max",
                FGMediationManager.FAILED_LOAD_PLACEMENT_NAME);
            MediationInstance.Callbacks._OnMrecAdFailedToLoad?.Invoke();
            FGMediationManager.Instance.Callbacks._OnMrecAdFailedToLoad?.Invoke();
        }

        protected override void TriggerDisplayFailedEventImpl()
        {
            CurrentCallback?.Invoke(false);
            _isMrecLoaded = false;
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.MREC, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
        }
    }
}
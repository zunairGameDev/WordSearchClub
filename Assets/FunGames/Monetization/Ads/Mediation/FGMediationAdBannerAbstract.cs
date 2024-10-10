using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdBannerAbstract<M, MP> : FGMediationAdAbstract<M, MP>
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
               private bool _isBannerLoaded = false;
        private bool _showBannerAsked = false;
        private bool _isBannerShowing = false;

        public override FGAdType adType => FGAdType.Banner;
        
        protected abstract void LoadAd();
        protected abstract void ShowAd();
        protected abstract void HideAd();

        public override bool IsReady()
        {
            if (FunGamesSDK.IsNoAd(adType)) return false;
#if UNITY_EDITOR
            return true;
#endif
            return _isBannerLoaded;
        }

        protected override void LoadImpl()
        {
            if (_isShowRequested)
            {
                MediationInstance.Log("Subscribe to banner loaded event");
                FGMediation.Callbacks.OnBannerAdLoaded += ShowOnceLoaded;
            }
            else
            {
                FGMediation.Callbacks.OnBannerAdLoaded -= ShowOnceLoaded;
            }
            LoadAd();
        }

        protected override void ShowImpl()
        {
            FGMediationManager.Instance.IsShowBannerRequested = true;
            
            if (_isBannerShowing) MediationInstance.Log("Banner is already showing !");
            if (_showBannerAsked) MediationInstance.Log("Banner is already asked !");

            if (_isBannerShowing || _showBannerAsked) return;
            _showBannerAsked = true;

#if !UNITY_EDITOR
            _isBannerShowing = _isBannerLoaded;
#else
            _isBannerShowing = true;
#endif
#if !UNITY_EDITOR
            if (_isBannerLoaded)
#else
            if (true)
#endif
            {
                ShowAd();
                TriggerDisplayedEvent(ShowingAdInfo);
                FGMediation.Callbacks.OnBannerAdLoaded -= ShowOnceLoaded;
            }
            else
            {
                MediationInstance.Log("Subscribe to banner loaded event");
                FGMediation.Callbacks.OnBannerAdLoaded -= ShowOnceLoaded;
                FGMediation.Callbacks.OnBannerAdLoaded += ShowOnceLoaded;
            }
        }

        private void ShowOnceLoaded(FGAdInfo adInfo)
        {
            _showBannerAsked = false;
            Show(ShowingAdInfo.Placement, CurrentCallback);
        }

        protected override void HideImpl()
        {
            FGMediation.Callbacks.OnBannerAdLoaded -= ShowOnceLoaded;
            FGMediationManager.Instance.IsShowBannerRequested = false;
            if (_isBannerShowing) TriggerClosedEvent();
            HideAd();
            _isBannerShowing = false;
            _isShowing = false;
            _showBannerAsked = false;
        }

        // private void RefreshAdUnitList()
        // {
        //     FGMediationManager.Instance.AllAdUnits.Remove(this);
        //       FGMediationManager.Instance.AllAdUnits.Remove(this);
        // }

        protected override void TriggerLoadedEventImpl()
        {
            _isLoading = false;
            _isBannerLoaded = true;
            MediationInstance._loadedBannerAd = LoadedAdInfo;
            FGAnalytics.NewAdEvent(AdAction.Loaded, AdType.Banner, LoadedAdInfo.NetworkName,
                LoadedAdInfo.Placement);
            MediationInstance.Callbacks._OnBannerAdLoaded?.Invoke(LoadedAdInfo);
            FGMediationManager.Instance.Callbacks._OnBannerAdLoaded?.Invoke(LoadedAdInfo);
        }

        protected override void TriggerDisplayedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Show, AdType.Banner, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
        }

        protected override void TriggerClosedEventImpl()
        {
            MediationInstance._loadedBannerAd = ShowingAdInfo;
            FGAnalytics.NewAdEvent(AdAction.Dismissed, AdType.Banner, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
        }

        protected override void TriggerClickedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Clicked, AdType.Banner, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnBannerAdClicked?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnBannerAdClicked?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerImpressionEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Impression, AdType.Banner, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnBannerAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnBannerAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
        }

        protected override void TriggerLoadFailedEventImpl()
        {
            _isBannerLoaded = false;
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.Banner, "Max",
                FGMediationManager.FAILED_LOAD_PLACEMENT_NAME);
            MediationInstance.Callbacks._OnBannerAdFailedToLoad?.Invoke();
            FGMediationManager.Instance.Callbacks._OnBannerAdFailedToLoad?.Invoke();
        }

        protected override void TriggerDisplayFailedEventImpl()
        {
            // Do nothing.
        }
    }
}
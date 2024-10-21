using System;
using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdInterstitialAbstract<M, MP> : FGMediationAdAbstract<M, MP>
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
        public override FGAdType adType => FGAdType.Interstitial;

        protected abstract void ShowAd(string placementName);

        protected override void ShowImpl()
        {
            try
            {
                ShowAd(ShowingAdInfo.Placement);
            }
            catch (Exception e)
            {
                FGAnalytics.NewDesignEvent("Error:UserQuitBeforeEndingAd");
                MediationInstance.LogError(e.StackTrace);
                throw;
            }
        }

        protected override void HideImpl()
        {
            // Do nothing
        }


        protected override void TriggerLoadedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Loaded, AdType.Interstitial, LoadedAdInfo.NetworkName,
                LoadedAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdLoaded?.Invoke(LoadedAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdLoaded?.Invoke(LoadedAdInfo);
        }

        protected override void TriggerDisplayedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Show, AdType.Interstitial, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdDisplayed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdDisplayed?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(true);
        }

        protected override void TriggerClosedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Dismissed, AdType.Interstitial, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdDismissed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdDismissed?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerClickedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Clicked, AdType.Interstitial, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdClicked?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdClicked?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerImpressionEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Impression, AdType.Interstitial, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
        }

        protected override void TriggerLoadFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.Interstitial, "Max",
                FGMediationManager.FAILED_LOAD_PLACEMENT_NAME);
            MediationInstance.Callbacks._OnInterstitialAdFailedToLoad?.Invoke();
            FGMediationManager.Instance.Callbacks._OnInterstitialAdFailedToLoad?.Invoke();
        }

        protected override void TriggerDisplayFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.Interstitial, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnInterstitialAdFailedToDisplay?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnInterstitialAdFailedToDisplay?.Invoke(ShowingAdInfo);
             CurrentCallback?.Invoke(false);
        }
    }
}
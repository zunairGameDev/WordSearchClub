using System;
using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdRewardedAbstract<M, MP> : FGMediationAdAbstract<M, MP>
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
        public override FGAdType adType => FGAdType.Rewarded;

        protected abstract void ShowAd();

        protected override void ShowImpl()
        {
            try
            {
                ShowAd();
                FGAnalytics.NewDesignEvent("Rewarded" + ShowingAdInfo.Placement + ":succeeded");
            }
            catch (Exception e)
            {
                FGAnalytics.NewDesignEvent("RewardedError" + ShowingAdInfo.Placement + ":UserQuitBeforeEndingAd");
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
            FGAnalytics.NewAdEvent(AdAction.Loaded, AdType.RewardedVideo, LoadedAdInfo.NetworkName,
                LoadedAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedLoaded?.Invoke(LoadedAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedLoaded?.Invoke(LoadedAdInfo);
        }

        protected override void TriggerDisplayedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Show, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedDisplayed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedDisplayed?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerClosedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Dismissed, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedDismissed?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedDismissed?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerClickedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Clicked, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedClicked?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedClicked?.Invoke(ShowingAdInfo);
        }

        protected override void TriggerImpressionEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.Impression, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedAdImpression?.Invoke(AdUnitId, ShowingAdInfo);
        }

        protected override void TriggerLoadFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.RewardedVideo, "Max",
                FGMediationManager.FAILED_LOAD_PLACEMENT_NAME);
            MediationInstance.Callbacks._OnRewardedFailedToLoad?.Invoke();
            FGMediationManager.Instance.Callbacks._OnRewardedFailedToLoad?.Invoke();
        }

        protected override void TriggerDisplayFailedEventImpl()
        {
            FGAnalytics.NewAdEvent(AdAction.FailedShow, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardedFailedToDisplay?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardedFailedToDisplay?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(false);
        }

        protected void TriggerRewardEvent()
        {
            if (!ShowingAdInfo.AdUnitIdentifier.Equals(AdUnitId)) return;

            MediationInstance.Log("Reward Received for " + ShowingAdInfo.AdUnitIdentifier + " - " +
                                  ShowingAdInfo.Placement);
            FGAnalytics.NewAdEvent(AdAction.RewardReceived, AdType.RewardedVideo, ShowingAdInfo.NetworkName,
                ShowingAdInfo.Placement);
            MediationInstance.Callbacks._OnRewardReceived?.Invoke(ShowingAdInfo);
            FGMediationManager.Instance.Callbacks._OnRewardReceived?.Invoke(ShowingAdInfo);
            CurrentCallback?.Invoke(true);
        }
    }
}
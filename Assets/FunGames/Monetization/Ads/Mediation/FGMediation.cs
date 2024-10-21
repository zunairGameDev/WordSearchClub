using System;

namespace FunGames.Mediation
{
    public class FGMediation
    {
        // public static string RC_A = "MediationConfigA";
        // public static string RC_A_MaxLoadingDelay = "MediationConfigAMaxLoadingDelay";
        // public static string RC_B = "MediationConfigB";

        public static FGMediationCallbacks Callbacks => FGMediationManager.Instance.Callbacks;

        public static bool IsBannerReady => FGMediationManager.Instance.IsBannerReady;
        public static bool IsInterstitialReady => FGMediationManager.Instance.IsInterstitialReady;
        public static bool IsRewardedReady => FGMediationManager.Instance.IsRewardedReady;
        public static bool IsMrecReady => FGMediationManager.Instance.IsMrecReady;
        public static bool IsAppOpenReady => FGMediationManager.Instance.IsAppOpenReady;

        public static float TimeSinceLastInterstitial => FGMediationManager.Instance.TimeSinceLastInterstitial;

        /// <summary>
        /// Display an Interstitial Ad
        /// </summary>
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowInterstitial(
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME, Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowInterstitial(displayCallback, placementName);

        /// <summary>
        /// Display an Interstitial Ad with a specific unitId
        /// </summary>
        /// <param name="unitId">Id of the Ad to show</param>
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowInterstitialWithId(string unitId,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME, Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowInterstitial(displayCallback, placementName, unitId);

        /// <summary>
        /// Display a Rewarded Ad
        /// </summary>
        /// <param name="rewardedCallback"> The action to perform when the user can receive the reward. The boolean corresponds to the status of the reward callback (success or fail) </param>
        /// <param name="placementName">The name of the Ad placement</param>
        public static void ShowRewarded(Action<bool> rewardedCallback,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME) =>
            FGMediationManager.Instance.ShowRewarded(rewardedCallback, placementName);

        /// <summary>
        /// Display a Rewarded Ad with a specific unitId
        /// </summary>
        /// <param name="unitId">Id of the Ad to show</param>
        /// <param name="rewardedCallback"> The action to perform when the user can receive the reward. The boolean corresponds to the status of the reward callback (success or fail) </param>
        /// <param name="placementName">The name of the Ad placement</param>
        public static void ShowRewardedWithId(string unitId, Action<bool> rewardedCallback,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME) =>
            FGMediationManager.Instance.ShowRewarded(rewardedCallback, placementName, unitId);

        /// <summary>
        /// Display a Banner Ad
        /// </summary>
        /// <param name="placementName">The name of the Ad placement</param>
        public static void ShowBanner(string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME) =>
            FGMediationManager.Instance.ShowBanner(placementName);

        /// <summary>
        /// Display a Banner Ad with a specific unitId
        /// </summary>
        /// <param name="unitId">Id of the Ad to show</param>
        /// <param name="placementName">The name of the Ad placement</param>
        public static void ShowBannerWithId(string unitId,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME) =>
            FGMediationManager.Instance.ShowBanner(placementName, unitId);

        /// <summary>
        /// Close current Banner Ad
        /// </summary>
        public static void HideBanner() => FGMediationManager.Instance.HideBanner();

        /// <summary>
        ///  Display an App Open Ad
        /// </summary>
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowAppOpen(string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME,
            Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowAppOpen(displayCallback, placementName);

        /// <summary>
        /// Display an App Open Ad with a specific unitId
        /// </summary>
        /// <param name="unitId">Id of the Ad to show</param
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowAppOpenWithId(string unitId,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME, Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowAppOpen(displayCallback, placementName, unitId);

        public static void AddAppOpenCondition(Func<bool> condition) =>
            FGMediationManager.Instance.AddAppOpenCondition(condition);

        /// <summary>
        /// Display a Mrec Ad
        /// </summary>
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowMrec(string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME,
            Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowMrec(displayCallback, placementName);

        /// <summary>
        /// Display a Mrec Ad with a specific unitId
        /// </summary>
        /// <param name="unitId">Id of the Ad to show</param>
        /// <param name="placementName">The name of the Ad placement</param>
        /// <param name="displayCallback">Callback triggered when ad display failed or succeeded</param>
        public static void ShowMrecWithId(string unitId,
            string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME, Action<bool> displayCallback = null) =>
            FGMediationManager.Instance.ShowMrec(displayCallback, placementName, unitId);

        public static void HideMrec() => FGMediationManager.Instance.HideMrec();

        // public void OverrideAppOpenCondition(Func<bool> condition) => FGMediationManager.Instance.OverrideAppOpenCondition(condition);
    }
}
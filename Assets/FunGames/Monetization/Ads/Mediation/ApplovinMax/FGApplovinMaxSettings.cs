using System;
using System.Collections.Generic;
using FunGames.Core.Utils;
using FunGames.Tools.Utils;
using UnityEngine;

namespace FunGames.Mediation.ApplovinMax
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGApplovinMaxSettings : FGModuleSettingsAbstract<FGApplovinMaxSettings>
    {
        public const string NAME = "FGApplovinMaxSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGApplovinMaxSettings LoadResources()
        {
            return Resources.Load<FGApplovinMaxSettings>(PATH);
        }

        // [Header("Applovin Max")]

        //[Tooltip("Use Max")]
        //public bool useMax;
        [Tooltip("Max Sdk Key")] public string maxSdkKey =
            "-x3h7mcZ5EdJJCd0iDab_rNf-6t9bsentb_ilJcaZ_ORIGB0P4reTeRrMeRe39-EAu-F6Bqcgah9fv-gSdoO1U";

        [Header("Default iOS Units")] public string iOSInterstitialAdUnitId = String.Empty;
        public string iOSRewardedAdUnitId = String.Empty;
        public string iOSBannerAdUnitId = String.Empty;
        public string iOSAppOpenAdUnitId = String.Empty;
        public string iOSMrecAdUnitId = String.Empty;

        [Header("Default Android Units")] public string androidInterstitialAdUnitId = String.Empty;
        public string androidRewardedAdUnitId = String.Empty;
        public string androidBannerAdUnitId = String.Empty;
        public string androidAppOpenAdUnitId = String.Empty;
        public string androidMrecAdUnitId = String.Empty;

        [Header("Default Amazon Units")] public string amazonInterstitialAdUnitId = String.Empty;
        public string amazonRewardedAdUnitId = String.Empty;
        public string amazonBannerAdUnitId = String.Empty;
        public string amazonAppOpenAdUnitId = String.Empty;
        public string amazonMrecAdUnitId = String.Empty;

        [Header("Banner Settings")] public Color BannerBackgroundColor = Color.black;

        [Header("")] public List<FGAdUnit> AdditionalUnitIds = new List<FGAdUnit>();

        public string DefaultInterstitialId
        {
            get =>
#if UNITY_IOS
                iOSInterstitialAdUnitId;
#else
                CurrentPlatform.IsFireOS ? amazonInterstitialAdUnitId : androidInterstitialAdUnitId;
#endif
        }

        public string DefaultRewardedId
        {
            get =>
#if UNITY_IOS
                iOSRewardedAdUnitId;
#else
                CurrentPlatform.IsFireOS ? amazonRewardedAdUnitId : androidRewardedAdUnitId;
#endif
        }

        public string DefaultBannerId
        {
            get =>
#if UNITY_IOS
                iOSBannerAdUnitId;
#else
                CurrentPlatform.IsFireOS ? amazonBannerAdUnitId : androidBannerAdUnitId;
#endif
        }

        public string DefaultAppOpenId
        {
            get =>
#if UNITY_IOS
                iOSAppOpenAdUnitId;
#else
                CurrentPlatform.IsFireOS ? amazonAppOpenAdUnitId : androidAppOpenAdUnitId;
#endif
        }

        public string DefaultMrecId
        {
            get =>
#if UNITY_IOS
                iOSMrecAdUnitId;
#else
                CurrentPlatform.IsFireOS ? amazonMrecAdUnitId : androidMrecAdUnitId;
#endif
        }
    }
}
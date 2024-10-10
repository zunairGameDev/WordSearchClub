using com.adjust.sdk;
using System;
using System.Text;
using FunGames.Analytics;
using FunGames.UserConsent;
using FunGames.Core;
using FunGames.Core.Modules;
using FunGames.Mediation;
using FunGames.RemoteConfig;
using FunGames.Tools.Utils;
using UnityEngine;

namespace FunGames.MMP.AdjustMMP
{
    public class FGAdjust : FGModuleAbstract<FGAdjust, FGMMPCallbacks, FGAdjustSettings>
    {
        public override FGAdjustSettings Settings => FGAdjustSettings.settings;
        protected override FGModule Parent => FGMMPManager.Instance;
        protected override string EventName => "Adjust";
        protected override string RemoteConfigKey => "FGAdjust";
        
        private const string RC_ADJUST_SANDBOX = "AdjustSandbox";

        public const string RC_REV_EVENT_FREQ = "RevenueEventFrequency";
        public const string RC_REV_AMOUNT_MULT = "RevenueAmountMultiplier";
        public const string RC_REV_AMOUNT_INCR = "RevenueAmountIncrementer";

        private const string PP_PLAYTIME = "playtime";
        private const string PP_NB_SESSION = "nbSession";
        private const string PP_NB_RV = "nbRV";
        private const string PP_SESSION_LENGHT = "sessionLenght";

        private const string PP_ADJUST_TOKEN_RET1 = "AdjustTokenRet1";
        private const string PP_ADJUST_TOKEN_RET3 = "AdjustTokenRet3";
        private const string PP_ADJUST_TOKEN_RET5 = "AdjustTokenRet5";
        private const string PP_ADJUST_TOKEN_RET7 = "AdjustTokenRet7";

        private int daysSinceFirstConnection;
        private int daysSinceLastConnection;
        private int nbSessionToday;
        private int nbRV;

        private int _revenueEventCounter = 0;
        private double _revenuAmountCounter = 0;

        private int _revenueEventFrequency;
        private double _revenueAmountMultiplier;
        private double _revenueAmountIncrementer;

        private Action<string, FGAdInfo> _interImpressionCallback;
        private Action<string, FGAdInfo> _bannerImpressionCallback;
        private Action<string, FGAdInfo> _rewardedImpressionCallback;
        private Action<string, FGAdInfo> _crosspromoImpressionCallback;
        private Action<string, FGAdInfo> _mrecImpressionCallback;
        private Action<string, FGAdInfo> _appOpenImpressionCallback;

        private static bool _subscribed = false;

        protected override void InitializeCallbacks()
        {
            FGRemoteConfig.AddDefaultValue(RC_ADJUST_SANDBOX, 0);

            FGRemoteConfig.AddDefaultValue(RC_REV_EVENT_FREQ, 1);
            FGRemoteConfig.AddDefaultValue(RC_REV_AMOUNT_MULT, 1);
            FGRemoteConfig.AddDefaultValue(RC_REV_AMOUNT_INCR, 0);

            FGMMPManager.Instance.Callbacks.Initialization += Initialize;
            FGRemoteConfig.Callbacks.OnInitialized += UpdateRemoteConfValues;

            _interImpressionCallback = (adUnitId, adInfo) => runCallback("INTER", adInfo);
            _bannerImpressionCallback = (adUnitId, adInfo) => runCallback("BANNER", adInfo);
            _rewardedImpressionCallback = (adUnitId, adInfo) => runCallback("REWARDED", adInfo);
            _crosspromoImpressionCallback = (adUnitId, adInfo) => runCallback("XPROMO", adInfo);
            _mrecImpressionCallback = (adUnitId, adInfo) => runCallback("MREC", adInfo);
            _appOpenImpressionCallback= (adUnitId, adInfo) => runCallback("APPOPEN", adInfo);

            FGMediation.Callbacks.OnInterstitialAdImpression += _interImpressionCallback;
            FGMediation.Callbacks.OnBannerAdImpression += _bannerImpressionCallback;
            FGMediation.Callbacks.OnRewardedAdImpression += _rewardedImpressionCallback;
            FGMediation.Callbacks.OnCrosspromoAdImpression += _crosspromoImpressionCallback;
            FGMediation.Callbacks.OnMrecAdImpression += _mrecImpressionCallback;
            FGMediation.Callbacks.OnAppOpenAdImpression += _appOpenImpressionCallback;
            // FGMediation.Callbacks.OnRvInterAdRevenuePaidEvent += (adUnitId, adInfo) => runCallback("REWARDED_INTER", adInfo);
            _subscribed = true;
        }

        protected override void OnAwake()
        {
            // throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new NotImplementedException();
        }

        protected override void InitializeModule()
        {
            AdjustEnvironment environment = GetEnvironment();
            Log("Environment : " + environment);

            Adjust.addSessionCallbackParameter("user_cohort",
                FGRemoteConfig.CurrentABTest != null ? FGRemoteConfig.CurrentABTest.Id : "none");
            Adjust.addSessionCallbackParameter("FunGamesSDK", FGCore.Instance.ModuleInfo.Version);

            AdjustConfig adjustConfig = new AdjustConfig(FGAdjustSettings.settings.AppToken.Trim(), environment);
            adjustConfig.setLogLevel(FGAdjustSettings.settings.logLevel);
            adjustConfig.setAttributionChangedDelegate(attributionChangedDelegate);
            adjustConfig.setPreinstallTrackingEnabled(true);
            adjustConfig.setSendInBackground(FGAdjustSettings.settings.sendInBackground);
            adjustConfig.setDeferredDeeplinkDelegate(OnDeferredDeepLink);
            Adjust.start(adjustConfig);
            LocalisationUtils.GetLocalisationCode((location) =>
            {
                AdjustThirdPartySharing adjustThirdPartySharing =
                    new AdjustThirdPartySharing(FGUserConsent.GdprStatus.TargetedAdvertisingAccepted);
                adjustThirdPartySharing.addGranularOption("google_dma", "eea", LocalisationUtils.isEEA(location) ? "1" : "0");
                adjustThirdPartySharing.addGranularOption("google_dma", "ad_personalization", FGUserConsent.GdprStatus.IsFullyAccepted ? "1" : "0");
                adjustThirdPartySharing.addGranularOption("google_dma", "ad_user_data", FGUserConsent.GdprStatus.IsFullyAccepted ? "1" : "0");
                Adjust.trackThirdPartySharing(adjustThirdPartySharing);
            });

            if (!FGUserConsent.GdprStatus.AnalyticsAccepted) Adjust.trackMeasurementConsent(false);

            InitializationComplete(!String.IsNullOrEmpty(FGAdjustSettings.settings.AppToken));

            if (String.IsNullOrEmpty(FGAdjustSettings.settings.AppToken))
                LogError("App Token is missing in FG Adjust Settings");

            InitializePlayerPrefs();
        }


        private void InitializePlayerPrefs()
        {
            if (FGCore.Instance.IsFirstConnection()) PlayerPrefs.SetInt(PP_PLAYTIME, 0);

            // Retention
            daysSinceFirstConnection = FGCore.Instance.DaysSinceFirstConnection();
            daysSinceLastConnection = FGCore.Instance.DaysSinceLastConnection();

            if (daysSinceFirstConnection == 1)
            {
                if (!PlayerPrefs.HasKey(PP_ADJUST_TOKEN_RET1))
                {
                    PlayerPrefs.SetInt(PP_ADJUST_TOKEN_RET1, 1);
                    AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustTokenRet1);
                    Adjust.trackEvent(adjustEvent);
                }
            }
            else if (daysSinceFirstConnection == 3)
            {
                if (!PlayerPrefs.HasKey(PP_ADJUST_TOKEN_RET3))
                {
                    PlayerPrefs.SetInt(PP_ADJUST_TOKEN_RET3, 3);
                    AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustTokenRet3);
                    Adjust.trackEvent(adjustEvent);
                }
            }
            else if (daysSinceFirstConnection == 5)
            {
                if (!PlayerPrefs.HasKey(PP_ADJUST_TOKEN_RET5))
                {
                    PlayerPrefs.SetInt(PP_ADJUST_TOKEN_RET5, 5);
                    AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustTokenRet5);
                    Adjust.trackEvent(adjustEvent);
                }
            }
            else if (daysSinceFirstConnection == 7)
            {
                if (!PlayerPrefs.HasKey(PP_ADJUST_TOKEN_RET7))
                {
                    PlayerPrefs.SetInt(PP_ADJUST_TOKEN_RET7, 7);
                    AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustTokenRet7);
                    Adjust.trackEvent(adjustEvent);
                }
            }

            // NB Session
            // if (PlayerPrefs.HasKey(PP_DATE_LAST_CO))
            // {
            //     DateTime store = Convert.ToDateTime(PlayerPrefs.GetString(PP_DATE_LAST_CO), CultureInfo.InvariantCulture);
            //     PlayerPrefs.SetString(PP_DATE_LAST_CO, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            //     DateTime today = DateTime.Now;
            //
            //     TimeSpan elapsed = today.Subtract(store);
            //     daysSinceLastConnection = (int)elapsed.TotalDays;
            // }
            // else
            // {
            //     PlayerPrefs.SetString(PP_DATE_LAST_CO, DateTime.Now.ToString(CultureInfo.InvariantCulture));
            //     daysSinceLastConnection = 0;
            // }


            if (PlayerPrefs.HasKey(PP_NB_SESSION))
            {
                if (daysSinceLastConnection != 0)
                {
                    PlayerPrefs.SetInt(PP_NB_SESSION, 0);
                    nbSessionToday = 0;
                }
                else
                {
                    nbSessionToday = PlayerPrefs.GetInt(PP_NB_SESSION);
                    nbSessionToday = nbSessionToday + 1;
                    PlayerPrefs.SetInt(PP_NB_SESSION, nbSessionToday);
                }
            }
            else
            {
                PlayerPrefs.SetInt(PP_NB_SESSION, 0);
                nbSessionToday = 0;
            }


            /*if (PlayerPrefs.HasKey("retention"))
            {
                PlayerPrefs.SetInt("retention", PlayerPrefs.GetInt("retention")+1);
            }
            else
            {
                PlayerPrefs.SetInt("retention", 0);
                nbSessionToday = 0;
            }*/

            if (!PlayerPrefs.HasKey(PP_NB_RV))
            {
                PlayerPrefs.SetInt(PP_NB_RV, 0);
            }

            PlayerPrefs.SetInt(PP_SESSION_LENGHT, 0);


            if (_subscribed)
            {
                Debug.Log("Ignoring duplicate adjust max subscription");
                return;
            }
        }

        private void runCallback(string format, FGAdInfo adInfo)
        {
            if (format == "INTER")
            {
                AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustEventInterID);
                Adjust.trackEvent(adjustEvent);
            }
            else if (format == "REWARDED")
            {
                nbRV = PlayerPrefs.GetInt(PP_NB_RV) + 1;
                PlayerPrefs.SetInt(PP_NB_RV, nbRV);
                AdjustEvent adjustEvent = new AdjustEvent(FGAdjustSettings.settings.AdjustEventRewardedID);
                Adjust.trackEvent(adjustEvent);
                if (nbRV == FGAdjustSettings.settings.nbRV1)
                {
                    Adjust.trackEvent(new AdjustEvent(FGAdjustSettings.settings.nbRV1Token));
                }
                else if (nbRV == FGAdjustSettings.settings.nbRV2)
                {
                    Adjust.trackEvent(new AdjustEvent(FGAdjustSettings.settings.nbRV2Token));
                }

                else if (nbRV == FGAdjustSettings.settings.nbRV3)
                {
                    Adjust.trackEvent(new AdjustEvent(FGAdjustSettings.settings.nbRV3Token));
                }

                else if (nbRV == FGAdjustSettings.settings.nbRV4)
                {
                    Adjust.trackEvent(new AdjustEvent(FGAdjustSettings.settings.nbRV4Token));
                }

                else if (nbRV == FGAdjustSettings.settings.nbRV5)
                {
                    Adjust.trackEvent(new AdjustEvent(FGAdjustSettings.settings.nbRV5Token));
                }
            }

            TrackRevenue(adInfo);
        }

        public void attributionChangedDelegate(AdjustAttribution attribution)
        {
            Log("Attribution changed");
            FGAttributionInfo attributionInfo = new FGAttributionInfo.Builder()
                .SetNetwork(attribution.network)
                .SetCampaign(attribution.campaign)
                .SetAdGroup(attribution.adgroup)
                .SetCreative(attribution.creative)
                .SetTrackerName(attribution.trackerName)
                .SetTrackerToken(attribution.trackerToken)
                .Build();
            
            FGAnalytics.NewDesignEvent("NetworkAttribution:" + attribution.network);
            Callbacks._onAttributionChanged?.Invoke(attributionInfo);
            FGMMPManager.Instance.Callbacks._onAttributionChanged?.Invoke(attributionInfo);
        }

        public static AdjustEnvironment GetEnvironment()
        {
            return FGRemoteConfig.GetBooleanValue(RC_ADJUST_SANDBOX)
                ? AdjustEnvironment.Sandbox
                : AdjustEnvironment.Production;
        }

        private void TrackRevenue(FGAdInfo adInfo)
        {
            _revenueEventCounter++;
            _revenuAmountCounter += adInfo.Revenue;

            SendSimpleRevenueEvent(adInfo);

            if (!_revenueEventCounter.Equals(_revenueEventFrequency)) return;

            SendAggregatedRevenueEvent(adInfo);

            _revenueEventCounter = 0;
            _revenuAmountCounter = 0;
        }

        private void SendSimpleRevenueEvent(FGAdInfo adInfo)
        {
            AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX);
            // set revenue and currency
            adjustAdRevenue.setRevenue(adInfo.Revenue, "USD");
            // optional parameters
            adjustAdRevenue.setAdRevenueNetwork(adInfo.NetworkName);
            adjustAdRevenue.setAdRevenueUnit(adInfo.AdUnitIdentifier);
            adjustAdRevenue.setAdRevenuePlacement(adInfo.Placement);
            // track ad revenue
            Adjust.trackAdRevenue(adjustAdRevenue);
        }

        private void SendAggregatedRevenueEvent(FGAdInfo adInfo)
        {
            // initialise with AppLovin MAX source
            AdjustAdRevenue adjustAdRevenueAggregated =
                new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAppLovinMAX + "_aggregated");
            double revenue = (_revenueAmountMultiplier * _revenuAmountCounter) + (_revenueAmountIncrementer / 100000);
            // set revenue and currency
            adjustAdRevenueAggregated.setRevenue(revenue, "USD");
            // optional parameters
            bool isDefaultTracking = 1.Equals(_revenueEventFrequency) && 1.Equals((int)_revenueAmountMultiplier) &&
                                     0.Equals((int)_revenueAmountIncrementer);
            adjustAdRevenueAggregated.setAdRevenueNetwork(isDefaultTracking ? adInfo.NetworkName : "aggregated");
            adjustAdRevenueAggregated.setAdRevenueUnit(isDefaultTracking ? adInfo.AdUnitIdentifier : "aggregated");
            adjustAdRevenueAggregated.setAdRevenuePlacement(isDefaultTracking ? adInfo.Placement : "aggregated");

            // track ad revenue
            Adjust.trackAdRevenue(adjustAdRevenueAggregated);

            StringBuilder logBuilder = new StringBuilder();
            logBuilder.Append("TRACK EVENT :");
            logBuilder.Append(" - frequency : " + _revenueEventFrequency);
            logBuilder.Append(" - multiplier : " + _revenueAmountMultiplier);
            logBuilder.Append(" - incrementer : " + _revenueAmountIncrementer);
            logBuilder.Append(" - aggregate revenue : " + _revenuAmountCounter);
            logBuilder.Append("Final revenue " + revenue);
            Log(logBuilder.ToString());
        }

        private void UpdateRemoteConfValues(bool res)
        {
            _revenueEventFrequency = FGRemoteConfig.GetIntValue(RC_REV_EVENT_FREQ);
            _revenueAmountMultiplier = FGRemoteConfig.GetDoubleValue(RC_REV_AMOUNT_MULT);
            _revenueAmountIncrementer = FGRemoteConfig.GetDoubleValue(RC_REV_AMOUNT_INCR);
        }
        
        private void OnDeferredDeepLink(string obj)
        {
            Callbacks._onDeferredDeepLink?.Invoke(obj);
            FGMMPManager.Instance.Callbacks._onDeferredDeepLink?.Invoke(obj);
        }

        protected override void ClearInitialization()
        {
            FGMMPManager.Instance.Callbacks.Initialization -= Initialize;
            FGMediation.Callbacks.OnInterstitialAdImpression -= _interImpressionCallback;
            FGMediation.Callbacks.OnBannerAdImpression -= _bannerImpressionCallback;
            FGMediation.Callbacks.OnRewardedAdImpression -= _rewardedImpressionCallback;
            FGMediation.Callbacks.OnCrosspromoAdImpression -= _crosspromoImpressionCallback;
            FGMediation.Callbacks.OnMrecAdImpression -= _mrecImpressionCallback;
            FGMediation.Callbacks.OnAppOpenAdImpression -= _appOpenImpressionCallback;
        }
        //#endif
    }
}
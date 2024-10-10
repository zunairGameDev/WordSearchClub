using System;
using Firebase;
using Firebase.Analytics;
using System.Collections.Generic;
using System.Text;
using FunGames.UserConsent;
using FunGames.Core.Modules;
using FunGames.Mediation;

namespace FunGames.Analytics.FirebaseA
{
    public class FGFireBase : FGAnalyticsAbstract<FGFireBase, FGAnalyticsCallbacks, FGFirebaseSettings>
    {
        public override FGFirebaseSettings Settings => FGFirebaseSettings.settings;
        protected override FGModule Parent => FGAnalyticsManager.Instance;

        protected override string EventName => "Firebase";
        protected override string RemoteConfigKey => "FGFirebase";

        private Action<string, FGAdInfo> _interImpressionCallback;
        private Action<string, FGAdInfo> _bannerImpressionCallback;
        private Action<string, FGAdInfo> _rewardedImpressionCallback;
        private Action<string, FGAdInfo> _crosspromoImpressionCallback;
        private Action<string, FGAdInfo> _mrecImpressionCallback;

        protected override void InitializeCallbacks()
        {
            base.InitializeCallbacks();
            SetMaxInitTime(8);
            _interImpressionCallback = (adUnitId, adInfo) => SendAdImpressionEvent("INTER", adInfo);
            _bannerImpressionCallback = (adUnitId, adInfo) => SendAdImpressionEvent("BANNER", adInfo);
            _rewardedImpressionCallback = (adUnitId, adInfo) => SendAdImpressionEvent("REWARDED", adInfo);
            _crosspromoImpressionCallback = (adUnitId, adInfo) => SendAdImpressionEvent("XPROMO", adInfo);
            _mrecImpressionCallback = (adUnitId, adInfo) => SendAdImpressionEvent("MREC", adInfo);

            FGMediation.Callbacks.OnInterstitialAdImpression += _interImpressionCallback;
            FGMediation.Callbacks.OnBannerAdImpression += _bannerImpressionCallback;
            FGMediation.Callbacks.OnRewardedAdImpression += _rewardedImpressionCallback;
            FGMediation.Callbacks.OnCrosspromoAdImpression += _crosspromoImpressionCallback;
            FGMediation.Callbacks.OnMrecAdImpression += _mrecImpressionCallback;

            FGFirebaseDependencyChecker.Instance.OnDependencyResolved += DependencyCheckCompleted;
        }

        protected override void OnAwake()
        {
            // throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new NotImplementedException();
        }


        protected override void Init()
        {
            if (FGFirebaseDependencyChecker.Instance.StatusChecked)
            {
                FGFirebaseDependencyChecker.Instance.OnDependencyResolved -= DependencyCheckCompleted;
                DependencyCheckCompleted(FGFirebaseDependencyChecker.Instance.Status);
            }
            FGFirebaseDependencyChecker.Instance.StartChecking();
        }

        private void DependencyCheckCompleted(DependencyStatus status)
        {
            if (DependencyStatus.Available.Equals(status))
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(FGUserConsent.GdprStatus.AnalyticsAccepted);
                InitializationComplete(true);
            }
            else
            {
                InitializationComplete(false);
            }
        }

        protected override void ProgressionEvent(LevelStatus levelStatus, string prog01,
            int score = FGAnalytics.NO_SCORE)
        {
            string paramValue = BuildProgressionParameterValue(score, prog01);
            SendProgressionEvent(levelStatus, paramValue);
        }

        protected override void ProgressionEvent(LevelStatus levelStatus, string prog01, string prog02,
            int score = FGAnalytics.NO_SCORE)
        {
            string paramValue = BuildProgressionParameterValue(score, prog01, prog02);
            SendProgressionEvent(levelStatus, paramValue);
        }

        protected override void ProgressionEvent(LevelStatus levelStatus, string prog01, string prog02, string prog03,
            int score = FGAnalytics.NO_SCORE)
        {
            string paramValue = BuildProgressionParameterValue(score, prog01, prog02, prog03);
            SendProgressionEvent(levelStatus, paramValue);
        }

        private void SendProgressionEvent(LevelStatus levelStatus, string paramValue)
        {
            string eventName = ValidEventName(levelStatus.ToString());
            FirebaseAnalytics.LogEvent(eventName, FirebaseAnalytics.ParameterLevelName, paramValue);
        }

        private string BuildProgressionParameterValue(int score = FGAnalytics.NO_SCORE, params string[] prog)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < prog.Length; i++)
            {
                if (i != 0) sb.Append(";");
                sb.Append(prog[i]);
            }

            if (score != FGAnalytics.NO_SCORE) sb.Append(";" + score);
            return sb.ToString();
        }

        protected override void DesignEventSimple(string eventId, float eventValue)
        {
            // Log("Sending Event");
            // Log("Is Initialized : " + IsInitialized());
            // Log("Dependecy Checked : " + FGFirebaseDependencyChecker.Instance.StatusChecked);
            // Log("Dependecy Status : " + FGFirebaseDependencyChecker.Instance.Status);
            FirebaseAnalytics.LogEvent(ValidEventName("DesignEvent:" + eventId), FirebaseAnalytics.ParameterValue,
                eventValue);
        }

        protected override void DesignEventDictio(string eventId, Dictionary<string, object> customFields)
        {
            // Log("Sending Event");
            // Log("Is Initialized : " + IsInitialized());
            // Log("Dependecy Checked : " + FGFirebaseDependencyChecker.Instance.StatusChecked);
            // Log("Dependecy Status : " + FGFirebaseDependencyChecker.Instance.Status);

            List<Parameter> parameters = new List<Parameter>();
            foreach (var field in customFields)
            {
                parameters.Add(new Parameter(ValidEventName(field.Key), ValidEventName(field.Value?.ToString())));
            }

            FirebaseAnalytics.LogEvent(ValidEventName(eventId), parameters.ToArray());
        }

        protected override void AdEvent(AdAction adAction, AdType adType, string adSdkName, string adPlacement)
        {
            // Log("Sending Event");
            // Log("Is Initialized : " + IsInitialized());
            // Log("Dependecy Checked : " + FGFirebaseDependencyChecker.Instance.StatusChecked);
            Log("Dependecy Status : " + FGFirebaseDependencyChecker.Instance.Status);
            string eventId = FGAnalytics.CreateEventId("Ad" + adAction, adType.ToString(), adPlacement);
            Parameter[] AdParameters =
            {
                new Firebase.Analytics.Parameter("sdkNetwork", adSdkName),
            };
            FirebaseAnalytics.LogEvent(ValidEventName(eventId), AdParameters);
        }

        private string ValidEventName(string eventName)
        {
            if (eventName == null) return String.Empty;
            eventName = eventName.Replace(":", "_");
            eventName = eventName.Replace("-", "_");
            eventName = eventName.Replace(" ", "_");
            return eventName;
        }

        protected override void ClearInitialization()
        {
            base.ClearInitialization();
            FGMediation.Callbacks.OnInterstitialAdImpression -= _interImpressionCallback;
            FGMediation.Callbacks.OnBannerAdImpression -= _bannerImpressionCallback;
            FGMediation.Callbacks.OnRewardedAdImpression -= _rewardedImpressionCallback;
            FGMediation.Callbacks.OnCrosspromoAdImpression -= _crosspromoImpressionCallback;
            FGMediation.Callbacks.OnMrecAdImpression -= _mrecImpressionCallback;
            FGFirebaseDependencyChecker.Instance.OnDependencyResolved -= DependencyCheckCompleted;
        }

        /**
        * This event is used to run UA campaigns ! DO NOT CHANGE !!!
        */
        private void SendAdImpressionEvent(string format, FGAdInfo adInfo)
        {
            if (!IsInitialized)
            {
                PoolEvents.Add(delegate { SendAdImpressionEvent(format, adInfo); });
                return;
            }

            Parameter[] AdParameters =
            {
                new Parameter("ad_platform", "AppLovin"),
                new Parameter("ad_source", adInfo.NetworkName),
                new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
                new Parameter("ad_format", format),
                new Parameter("currency", "USD"),
                new Parameter("value", adInfo.Revenue)
            };
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression, AdParameters);
            Log("Ad Impression event sent : " + format);
        }
    }
}
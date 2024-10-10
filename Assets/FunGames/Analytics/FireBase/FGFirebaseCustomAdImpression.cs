using System;
using Firebase;
using Firebase.Analytics;
using FunGames.Analytics.FirebaseA;
using FunGames.UserConsent;
using FunGames.Mediation;
using UnityEngine;

public class FGFirebaseCustomAdImpression : MonoBehaviour
{
    private Action<string, FGAdInfo> _interImpressionCallback;
    private Action<string, FGAdInfo> _bannerImpressionCallback;
    private Action<string, FGAdInfo> _rewardedImpressionCallback;
    private Action<string, FGAdInfo> _crosspromoImpressionCallback;
    private Action<string, FGAdInfo> _mrecImpressionCallback;
    private bool _isInit = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _interImpressionCallback = (adUnitId, adInfo) => SendCustomAdImpressionEvent("INTER", adInfo);
        _bannerImpressionCallback = (adUnitId, adInfo) => SendCustomAdImpressionEvent("BANNER", adInfo);
        _rewardedImpressionCallback = (adUnitId, adInfo) => SendCustomAdImpressionEvent("REWARDED", adInfo);
        _crosspromoImpressionCallback = (adUnitId, adInfo) => SendCustomAdImpressionEvent("XPROMO", adInfo);
        _mrecImpressionCallback = (adUnitId, adInfo) => SendCustomAdImpressionEvent("MREC", adInfo);

        FGMediation.Callbacks.OnInterstitialAdImpression += _interImpressionCallback;
        FGMediation.Callbacks.OnBannerAdImpression += _bannerImpressionCallback;
        FGMediation.Callbacks.OnRewardedAdImpression += _rewardedImpressionCallback;
        FGMediation.Callbacks.OnCrosspromoAdImpression += _crosspromoImpressionCallback;
        FGMediation.Callbacks.OnMrecAdImpression += _mrecImpressionCallback;

        FGFirebaseDependencyChecker.Instance.OnDependencyResolved += DependencyCheckCompleted;
    }

    private void DependencyCheckCompleted(DependencyStatus status)
    {
        if (DependencyStatus.Available.Equals(status))
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(FGUserConsent.GdprStatus.AnalyticsAccepted);
            _isInit = true;
        }
        else
        {
            _isInit = false;
        }
    }

    /**
     * This event is used to run UA campaigns ! DO NOT CHANGE !!!
     */
    private void SendCustomAdImpressionEvent(string format, FGAdInfo adInfo)
    {
        if (!_isInit)
        {
            FGFireBase.Instance.PoolEvents.Add(delegate { SendCustomAdImpressionEvent(format, adInfo); });
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

        FirebaseAnalytics.LogEvent("custom_ad_impression", AdParameters);
        FGFireBase.Instance.Log("Custom Ad Impression event sent : " + format);
    }
}
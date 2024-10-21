using System;
using System.Collections.Generic;
using System.Text;
using FunGames.Analytics;
//using FunGames.Analytics.FirebaseA;
using FunGames.Core.Utils;
using FunGames.RemoteConfig;
using FunGames.Tools.Utils;
using UnityEngine;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGMaxLoadEventSender : Singleton<FGMaxLoadEventSender>
    {
        private const string RC_SEND_MAX_LOADING_EVENTS = "SendMaxLoadingEvents";

        private const string EVENT_AD_LOADED = "MaxLoad";
        private const string EVENT_LOAD_ERROR = "MaxLoadError";
        private const string EVENT_WATERFALL_INFO = "Wf";
        private const string EVENT_NETWORK_INFO = "Nw";
        private const string EVENT_CREDENTIALS = "Cr";
        private const string EVENT_ERROR_INFO = "Er";

        public void Initialize()
        {
            FGRemoteConfig.AddDefaultValue(RC_SEND_MAX_LOADING_EVENTS, 0);

            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += SendAdLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += SendAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += SendAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadedEvent += SendAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += SendAdLoadedEvent;

            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += SendAdLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += SendAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += SendAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += SendAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += SendAdLoadFailedEvent;

            FGRemoteConfig.Callbacks.OnInitialized += delegate
            {
                if (1.Equals(FGRemoteConfig.GetIntValue(RC_SEND_MAX_LOADING_EVENTS))) return;
                StopSendingEvents();
            };
        }

        private void SendAdLoadedEvent(string unitID, MaxSdkBase.AdInfo adInfo)
        {
            try
            {
                MaxSdkBase.WaterfallInfo waterfallInfo = adInfo?.WaterfallInfo;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(adInfo?.Placement);
                stringBuilder.Append(adInfo?.Revenue);
                stringBuilder.Append(adInfo?.NetworkName);
                stringBuilder.Append(adInfo?.AdUnitIdentifier);
                stringBuilder.Append(adInfo?.AdFormat);
                stringBuilder.Append(adInfo?.NetworkPlacement);
                stringBuilder.Append(adInfo?.CreativeIdentifier);
                stringBuilder.Append(adInfo?.RevenuePrecision);
                stringBuilder.Append(adInfo?.DspName);
                stringBuilder.Append(waterfallInfo?.Name);
                stringBuilder.Append(DateTime.Now);

                string eventUniqueId = FGAPIHelpers.CreateUniqueId(stringBuilder);
                string MaxLoad = FGAnalytics.CreateEventId(EVENT_AD_LOADED, eventUniqueId);
                string MaxLoad_Wf = FGAnalytics.CreateEventId(EVENT_AD_LOADED, EVENT_WATERFALL_INFO, eventUniqueId);

                //FGFireBase.Instance.SendDesignEventDictio(MaxLoad, GetLoadedAdInfoParameter(adInfo));
                //FGFireBase.Instance.SendDesignEventDictio(MaxLoad_Wf, GetWaterfallInfoParameters(waterfallInfo));
                for (int i = 0; i < waterfallInfo?.NetworkResponses?.Count; i++)
                {
                    MaxSdkBase.NetworkResponseInfo networkResponseInfo = waterfallInfo.NetworkResponses[i];
                    string MaxLoad_Wf_Nw = FGAnalytics.CreateEventId(EVENT_AD_LOADED, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, eventUniqueId);
                    string MaxLoad_Wf_Nw_Cr = FGAnalytics.CreateEventId(EVENT_AD_LOADED, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, EVENT_CREDENTIALS, eventUniqueId);
                    string MaxLoad_Wf_Nw_Er = FGAnalytics.CreateEventId(EVENT_AD_LOADED, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, EVENT_ERROR_INFO, eventUniqueId);
                    // Send event "MaxLoadedAdInfo:WaterfallInfo:NetworkInfo<i>:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoad_Wf_Nw,
                    //    GetNetworkResponseInfoParameters(networkResponseInfo));
                    //// Send event "MaxLoadedAdInfo:WaterfallInfo:NetworkInfo<i>:Credentials:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoad_Wf_Nw_Cr,
                    //    networkResponseInfo?.Credentials);
                    //// Send event "MaxLoadedAdInfo:WaterfallInfo:NetworkInfo<i>:ErrorInfo:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoad_Wf_Nw_Er,
                    //    GetErrorInfoParameters(networkResponseInfo?.Error));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[MAX EVENT LOADER]" + e);
            }
        }

        private void SendAdLoadFailedEvent(string unitID, MaxSdkBase.ErrorInfo errorInfo)
        {
            try
            {
                MaxSdkBase.WaterfallInfo waterfallInfo = errorInfo?.WaterfallInfo;

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(unitID);
                stringBuilder.Append(errorInfo?.Code);
                stringBuilder.Append(errorInfo?.Message);
                stringBuilder.Append(errorInfo?.AdLoadFailureInfo);
                stringBuilder.Append(errorInfo?.MediatedNetworkErrorCode);
                stringBuilder.Append(errorInfo?.MediatedNetworkErrorMessage);
                stringBuilder.Append(waterfallInfo?.Name);
                stringBuilder.Append(DateTime.Now);

                string eventUniqueId = FGAPIHelpers.CreateUniqueId(stringBuilder);
                string MaxLoadError = FGAnalytics.CreateEventId(EVENT_LOAD_ERROR, eventUniqueId);
                string MaxLoadError_Wf =
                    FGAnalytics.CreateEventId(EVENT_LOAD_ERROR, EVENT_WATERFALL_INFO, eventUniqueId);

                //// Send event "MaxLoadErrorInfo:<unique-id>"
                //FGFireBase.Instance.SendDesignEventDictio(MaxLoadError, GetErrorInfoParameters(errorInfo));
                //// Send event "MaxLoadErrorInfo:WaterfallInfo:<unique-id>"
                //FGFireBase.Instance.SendDesignEventDictio(MaxLoadError_Wf, GetWaterfallInfoParameters(waterfallInfo));
                for (int i = 0; i < waterfallInfo?.NetworkResponses?.Count; i++)
                {
                    MaxSdkBase.NetworkResponseInfo networkResponseInfo = waterfallInfo.NetworkResponses[i];

                    string MaxLoadError_Wf_Nw = FGAnalytics.CreateEventId(EVENT_LOAD_ERROR, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, eventUniqueId);
                    string MaxLoadError_Wf_Nw_Cr = FGAnalytics.CreateEventId(EVENT_LOAD_ERROR, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, EVENT_CREDENTIALS, eventUniqueId);
                    string MaxLoadError_Wf_Nw_Er = FGAnalytics.CreateEventId(EVENT_LOAD_ERROR, EVENT_WATERFALL_INFO,
                        EVENT_NETWORK_INFO + i, EVENT_ERROR_INFO, eventUniqueId);
                    
                    //// Send event "MaxLoadErrorInfo:WaterfallInfo:NetworkInfo<i>:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoadError_Wf_Nw,
                    //    GetNetworkResponseInfoParameters(networkResponseInfo));
                    //// Send event "MaxLoadErrorInfo:WaterfallInfo:NetworkInfo<i>:Credentials:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoadError_Wf_Nw_Cr,
                    //    networkResponseInfo?.Credentials);
                    //// Send event "MaxLoadErrorInfo:WaterfallInfo:NetworkInfo<i>:ErrorInfo:<unique-id>"
                    //FGFireBase.Instance.SendDesignEventDictio(MaxLoadError_Wf_Nw_Er,
                    //    GetErrorInfoParameters(networkResponseInfo?.Error));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[MAX EVENT LOADER]" + e);
            }
        }

        private Dictionary<string, object> GetLoadedAdInfoParameter(MaxSdkBase.AdInfo adInfo)
        {
            return new Dictionary<string, object>()
            {
                { "placement", adInfo?.Placement },
                { "revenue", adInfo?.Revenue },
                { "networkName", adInfo?.NetworkName },
                { "adUnitId", adInfo?.AdUnitIdentifier },
                { "adFormat", adInfo?.AdFormat },
                { "networkPlacement", adInfo?.NetworkPlacement },
                { "creativeId", adInfo?.CreativeIdentifier },
                { "precision", adInfo?.RevenuePrecision },
                { "dspName", adInfo?.DspName }
            };
        }

        private Dictionary<string, object> GetWaterfallInfoParameters(MaxSdkBase.WaterfallInfo waterfallInfo)
        {
            return new Dictionary<string, object>()
            {
                { "name", waterfallInfo?.Name },
                { "testName", waterfallInfo?.TestName },
                { "latency", waterfallInfo?.LatencyMillis }
            };
        }

        private Dictionary<string, object> GetNetworkResponseInfoParameters(
            MaxSdkBase.NetworkResponseInfo networkResponseInfo)
        {
            return new Dictionary<string, object>()
            {
                { "name", networkResponseInfo?.MediatedNetwork?.Name },
                { "adapterVersion", networkResponseInfo?.MediatedNetwork?.AdapterVersion },
                { "sdkVersion", networkResponseInfo?.MediatedNetwork?.SdkVersion },
                { "adapterClassName", networkResponseInfo?.MediatedNetwork?.AdapterClassName },
                { "adLoadState", networkResponseInfo?.AdLoadState },
                { "isBidding", networkResponseInfo?.IsBidding },
                { "latency", networkResponseInfo?.LatencyMillis }
            };
        }

        private Dictionary<string, object> GetErrorInfoParameters(MaxSdkBase.ErrorInfo errorInfo)
        {
            return new Dictionary<string, object>()
            {
                { "code", errorInfo?.Code },
                { "message", errorInfo?.Message },
                { "adLoadFailureInfo", errorInfo?.AdLoadFailureInfo },
                { "mediatedNetworkErrorCode", errorInfo?.MediatedNetworkErrorCode },
                { "mediatedNetworkErrorMessage", errorInfo?.MediatedNetworkErrorMessage },
            };
        }

        private void StopSendingEvents()
        {
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent -= SendAdLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent -= SendAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadedEvent -= SendAdLoadedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadedEvent -= SendAdLoadedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadedEvent -= SendAdLoadedEvent;

            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent -= SendAdLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent -= SendAdLoadFailedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent -= SendAdLoadFailedEvent;
            MaxSdkCallbacks.MRec.OnAdLoadFailedEvent -= SendAdLoadFailedEvent;
            MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent -= SendAdLoadFailedEvent;
        }
    }
}
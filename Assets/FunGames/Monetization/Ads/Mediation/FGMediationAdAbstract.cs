using System;
using System.Collections;
using System.Collections.Generic;
using FunGames.Analytics;
using FunGames.Core;
using FunGames.Core.Settings;
using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.Mediation
{
    public abstract class FGMediationAdAbstract<M, MP> : MonoBehaviour, IFGMediationAd
        where M : FGMediationAdAbstract<M, MP>
        where MP : FGMediationAbstract<MP, IFGModuleSettings>
    {
        public string AdUnitId
        {
            get => _adId;
            set => _adId = value;
        }

        private string _adId = String.Empty;
        public abstract FGAdType adType { get; }

        protected const string EVENT_LOADING_TIME = "AdLoadingTime";
        protected const string EVENT_PARAM_TYPE = "type";
        protected const string EVENT_PARAM_UNIT_ID = "unitId";
        protected const string EVENT_PARAM_NETWORK = "network";
        protected const string EVENT_PARAM_PLACEMENT = "placement";
        protected const string EVENT_PARAM_TIME = "time";
        protected const string EVENT_PARAM_LOAD_ITERATION = "loadIteration";

        protected FGAdInfo LoadedAdInfo = new FGAdInfo();
        protected FGAdInfo ShowingAdInfo = new FGAdInfo();
        protected Action<bool> CurrentCallback = null;
        private int _loadRetryAttempt = 0;
        private int _adIteration = 0;
        private float _timeToLoad = 0;
        protected bool _isLoading = false;
        protected bool _isShowing = false;
        protected bool _isShowRequested = false;
        private bool _isFirstLoadRequested = false;
        private bool _interrupted = false;

        protected abstract FGMediationAbstract<MP, IFGModuleSettings> MediationInstance { get; }


        public static M Initialize(FGAdType adType, string adId)
        {
            if (String.IsNullOrEmpty(adId)) return null;

            GameObject obj = new GameObject();
            obj.name = typeof(M).Name;
            M instance = obj.AddComponent<M>();
            instance.AdUnitId = adId;
            if (FunGamesSDK.IsNoAd(adType))
            {
                FGMediationManager.Instance.LogWarning("Ad will not initialize : Ads removed.");
                return instance;
            }

            instance.InitializeCallbacks();
            return instance;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            FGMediationManager.Instance.AllAdUnits.Add(this);
            FGMediationManager.Instance.AllAdUnits.Sort(CompareByAdType);
        }

        public abstract void InitializeCallbacks();

        protected abstract void LoadImpl();

        protected abstract void ShowImpl();
        protected abstract void HideImpl();

        public abstract bool IsReady();

        protected abstract void TriggerLoadedEventImpl();
        protected abstract void TriggerDisplayedEventImpl();
        protected abstract void TriggerClosedEventImpl();
        protected abstract void TriggerClickedEventImpl();
        protected abstract void TriggerImpressionEventImpl();

        protected abstract void TriggerLoadFailedEventImpl();

        protected abstract void TriggerDisplayFailedEventImpl();

        public void Load()
        {
            if (FunGamesSDK.IsNoAd(adType))
            {
                MediationInstance.LogWarning("Ad will not load : Ads removed.");
                return;
            }

            if (_isLoading) return;
            if (_isFirstLoadRequested && IsReady()) return;
            if (_interrupted) return;

            try
            {
                _adIteration++;
                _timeToLoad = Time.time;
                _isFirstLoadRequested = true;
                _isLoading = true;

                MediationInstance.Log("Loading Ad : " + adType + ":" + AdUnitId);

                LoadedAdInfo = new FGAdInfo();
                LoadedAdInfo.AdUnitIdentifier = AdUnitId;
                LoadImpl();
            }
            catch
            {
                MediationInstance.LogError("Failed Load Ad : Please Check Ad Unit");
            }
        }

        public void Show(string placementName = FGMediationManager.DEFAULT_PLACEMENT_NAME, Action<bool> callback = null)
        {
            if (FunGamesSDK.IsNoAd(adType))
            {
                MediationInstance.LogWarning(adType + "Ad will not show : Ads removed.");
                return;
            }

            if (_interrupted) return;

            _isShowRequested = true;
            if (!IsReady() && !_isLoading)
            {
                if (FGAdType.Rewarded.Equals(adType))
                    FGAnalytics.NewDesignEvent("RewardedNoAd" + placementName + ":NoAdReady");

                Load();
                MediationInstance.Log("Ad is not ready : " + adType);
                return;
            }

            if (FGMediationManager.Instance.IsAdShowing(adType))
            {
                MediationInstance.Log("Ad is already showing : " + adType);
                return;
            }

            CurrentCallback = callback;
            ShowingAdInfo.Set(LoadedAdInfo);
            ShowingAdInfo.Placement = placementName;
            ShowingAdInfo.AdUnitIdentifier = AdUnitId;
            MediationInstance.Log("Show Ad requested : " + adType + " : " + ShowingAdInfo.Placement);
            ShowImpl();
        }

        public void Hide()
        {
            _isShowRequested = false;
            HideImpl();
        }

        public bool IsShowing() => _isShowing;

        protected void TriggerLoadedEvent(FGAdInfo adInfo)
        {
            if (!adInfo.AdUnitIdentifier.Equals(AdUnitId)) return;

            StopCoroutine(WaitToReloadAd());
            LoadedAdInfo.Set(adInfo);
            LoadedAdInfo.Placement = FGMediationManager.DEFAULT_PLACEMENT_NAME;
            _loadRetryAttempt = 0;
            _timeToLoad = Time.time - _timeToLoad;
            _isLoading = false;
            MediationInstance.Log("Ad Loaded: " + adType + " (" + LoadedAdInfo.Placement + "-" +
                                  LoadedAdInfo.AdUnitIdentifier + ")");
            // SendLoadingTimeEvent();
            TriggerLoadedEventImpl();
        }

        protected void TriggerDisplayedEvent(FGAdInfo adInfo)
        {
            if (adInfo == null || !adInfo.AdUnitIdentifier.Equals(AdUnitId)) return;
            _isShowing = true;
            MediationInstance.Log("Ad Displayed: " + adType + " (" + ShowingAdInfo.Placement + "-" +
                                  ShowingAdInfo.AdUnitIdentifier + ")");
            TriggerDisplayedEventImpl();
        }

        protected void TriggerImpressionEvent(FGAdInfo adInfo)
        {
            if (adInfo == null || !adInfo.AdUnitIdentifier.Equals(AdUnitId)) return;
            string placementName = ShowingAdInfo.Placement;
            ShowingAdInfo.Set(adInfo);
            ShowingAdInfo.Placement = placementName;
            ShowingAdInfo.AdUnitIdentifier = AdUnitId;
            MediationInstance.Log("Ad Impression validated: " + adType + " (" + ShowingAdInfo.Placement + "-" +
                                  ShowingAdInfo.AdUnitIdentifier + ")");
            TriggerImpressionEventImpl();
        }

        protected void TriggerClosedEvent()
        {
            if (ShowingAdInfo == null || !ShowingAdInfo.AdUnitIdentifier.Equals(AdUnitId)) return;
            ForceLoad(this);
            _isShowing = false;
            CurrentCallback = null;
            MediationInstance.Log("Ad Closed: " + adType + " (" + ShowingAdInfo.Placement + "-" +
                                  ShowingAdInfo.AdUnitIdentifier + ")");
            TriggerClosedEventImpl();
        }

        protected void TriggerClickedEvent()
        {
            if (ShowingAdInfo == null || !ShowingAdInfo.AdUnitIdentifier.Equals(AdUnitId)) return;
            MediationInstance.Log("Ad Clicked: " + adType + " (" + ShowingAdInfo.Placement + "-" +
                                  ShowingAdInfo.AdUnitIdentifier + ")");
            TriggerClickedEventImpl();
        }

        protected void TriggerLoadFailedEvent()
        {
            if (LoadedAdInfo == null || !LoadedAdInfo.AdUnitIdentifier.Equals(AdUnitId)) return;

            _isLoading = false;
            StartCoroutine(WaitToReloadAd());

            MediationInstance.Log(
                "Ad Failed to load: " + adType + " (" + LoadedAdInfo.Placement + "-" + LoadedAdInfo.AdUnitIdentifier +
                ")");
            TriggerLoadFailedEventImpl();
        }

        protected void TriggerDisplayFailedEvent()
        {
            if (ShowingAdInfo == null || !ShowingAdInfo.AdUnitIdentifier.Equals(AdUnitId)) return;

            Load();
            CurrentCallback = null;
            MediationInstance.Log("Ad Failed to display: " + adType + " (" + ShowingAdInfo.Placement + "-" +
                                  ShowingAdInfo.AdUnitIdentifier + ")");
            TriggerDisplayFailedEventImpl();
        }

        private IEnumerator WaitToReloadAd()
        {
            if (!_interrupted)
            {
                _loadRetryAttempt++;
                double retryDelay = Math.Pow(2, _loadRetryAttempt);
                Debug.Log("Retry delay :" + retryDelay);
                yield return new WaitForSeconds((float)retryDelay);
                Load();
            }
        }

        private void SendLoadingTimeEvent()
        {
            SendLoadingTimeEvent(EVENT_LOADING_TIME, _timeToLoad);
        }

        protected void SendLoadingTimeEvent(string eventID, float timeToLoad)
        {
            // string uniqueId = FGAPIHelpers.CreateUniqueId(DateTime.Now.ToString(), LoadedAdInfo.AdFormat,
            //     LoadedAdInfo.Placement, LoadedAdInfo.NetworkName, LoadedAdInfo.AdUnitIdentifier, timeToLoad.ToString(),
            //     _adIteration.ToString());
            // string eventId = FGAnalytics.CreateEventId(eventID, LoadedAdInfo.AdFormat,
            //     LoadedAdInfo.Placement, LoadedAdInfo.NetworkName, uniqueId);
            FGAnalytics.NewDesignEvent(eventID, new Dictionary<string, object>
            {
                { EVENT_PARAM_TYPE, LoadedAdInfo.AdFormat },
                { EVENT_PARAM_PLACEMENT, LoadedAdInfo.Placement },
                { EVENT_PARAM_NETWORK, LoadedAdInfo.NetworkName },
                { EVENT_PARAM_UNIT_ID, LoadedAdInfo.AdUnitIdentifier },
                { EVENT_PARAM_TIME, timeToLoad },
                { EVENT_PARAM_LOAD_ITERATION, _adIteration },
            });
        }
        protected void ForceLoad(IFGMediationAd mediationAd)
        {
            FGMediationManager.Instance.CurrentLoadingAdIndex =
                FGMediationManager.Instance.AllAdUnits.IndexOf(mediationAd);
            mediationAd.Load();
        }

        private int CompareByAdType(IFGMediationAd x, IFGMediationAd y)
        {
            if (FGAdType.AppOpen.Equals(x.adType)) return -1;
            if (FGAdType.AppOpen.Equals(y.adType)) return 1;
            if (FGAdType.Banner.Equals(x.adType)) return -1;
            if (FGAdType.Banner.Equals(y.adType)) return 1;
            return 0;
        }

        public void InterruptAdLoading()
        {
            _interrupted = true;
            StopCoroutine(WaitToReloadAd());
        }

        public void ResetAdLoading()
        {
            _interrupted = false;
            Load();
        }
    }
}
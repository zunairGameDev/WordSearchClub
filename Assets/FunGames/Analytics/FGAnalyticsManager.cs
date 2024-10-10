using System;
using System.Collections.Generic;
using FunGames.UserConsent;
using FunGames.Core;
using FunGames.Core.Modules;

namespace FunGames.Analytics
{
    public class FGAnalyticsManager : FGModuleAbstract<FGAnalyticsManager, FGAnalyticsCallbacks, FGAnalyticsSettings>
    {
        public override FGAnalyticsSettings Settings => FGAnalyticsSettings.settings;
        protected override FGModule Parent => FGCore.Instance;
        protected override string EventName => "Analytics";

        protected override string RemoteConfigKey => "FGAnalytics";

        private Action _onInitialized;

        protected override void InitializeCallbacks()
        {
            _onInitialized = Initialize;
            FGUserConsent.OnComplete += _onInitialized;
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
            InitializationComplete(true);
        }

        public void SendProgressionEvent(LevelStatus levelStatus, string prog01, int score)
        {
            try
            {
                Callbacks._ProgressionEvent01?.Invoke(levelStatus, prog01, score);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Progression Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void SendProgressionEvent(LevelStatus levelStatus, string prog01, string prog02, int score)
        {
            try
            {
                Callbacks._ProgressionEvent02?.Invoke(levelStatus, prog01, prog02, score);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Progression Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void SendProgressionEvent(LevelStatus levelStatus, string prog01, string prog02, string prog03,
            int score)
        {
            try
            {
                Callbacks._ProgressionEvent03?.Invoke(levelStatus, prog01, prog02, prog03, score);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Progression Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void SendDesignEventSimple(string eventId, float eventValue)
        {
            try
            {
                Callbacks._DesignEventSimple?.Invoke(eventId, eventValue);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Design Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void SendDesignEventDictio(string eventId, Dictionary<string, object> customFields, float eventValue)
        {
            try
            {
                Callbacks._DesignEventDictio?.Invoke(eventId, customFields, eventValue);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Design Dico Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        public void SendAdEvent(AdAction adAction, AdType adType, string adSdkName, string adPlacement)
        {
            try
            {
                Callbacks._AdEvent?.Invoke(adAction, adType, adSdkName, adPlacement);
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while sending Ad Event :" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        protected override void ClearInitialization()
        {
            FGUserConsent.OnComplete -= _onInitialized;
        }
    }
}
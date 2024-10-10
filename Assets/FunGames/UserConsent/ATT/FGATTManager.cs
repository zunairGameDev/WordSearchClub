using System;
using FunGames.Core.Modules;
using UnityEngine;

namespace FunGames.UserConsent.ATT
{
    public class FGATTManager : FGModuleAbstract<FGATTManager, FGATTCallbacks, FGATTSettings>
    {
        public override FGATTSettings Settings => FGATTSettings.settings;
        protected override FGModule Parent => FGUserConsentManager.Instance;
        protected override string EventName => "ATT";
        protected override string RemoteConfigKey => "FGATT";

        [HideInInspector] public bool isATTCompliant = true;

        protected override void InitializeCallbacks()
        {
            InitWithoutTimer();
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

        public void ShowATT()
        {
            try
            {
                Callbacks?._show?.Invoke();
            }
            catch (Exception e)
            {
                LogCritical("An exception was raised while Displaying ATT:" + e.Message + "\n" +
                            e.StackTrace);
            }
        }

        protected override void ClearInitialization()
        {
            // FGRemoteConfig.Callbacks.OnInitialized -= _initialization;
        }
    }
}
using System;
using FunGames.Analytics;
using FunGames.UserConsent;
using FunGames.Core;
using FunGames.Core.Modules;
using FunGames.Tools.Debugging;
using UnityEngine;

namespace FunGames.MMP
{
    public class FGMMPManager : FGModuleAbstract<FGMMPManager, FGMMPCallbacks, FGMMPSettings>
    {
        public override FGMMPSettings Settings => FGMMPSettings.settings;
        protected override FGModule Parent => FGCore.Instance;
        protected override string EventName => "MMP";
        protected override string RemoteConfigKey => "FGMMP";

        private Action _initialization;

        protected override void InitializeCallbacks()
        {
            _initialization = Initialize;
            FGUserConsent.OnComplete += _initialization;
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

        protected override void ClearInitialization()
        {
            FGUserConsent.OnComplete -= _initialization;
        }
    }
}
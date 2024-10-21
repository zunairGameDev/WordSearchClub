using System;
using FunGames.Analytics;
using FunGames.Core.Modules;
using FunGames.Core.Settings;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.iOS;
#endif

namespace FunGames.UserConsent.ATT
{
    public abstract class FGATTAbstract<M, S> : FGModuleAbstract<M, FGATTCallbacks, S>
        where M : FGATTAbstract<M, S>
        where S : IFGModuleSettings
    {
        protected override FGModule Parent => FGATTManager.Instance;
        public abstract void ShowATT();

        protected override void InitializeCallbacks()
        {
            InitWithoutTimer();
            FGATTManager.Instance.Callbacks.Initialization += Initialize;
            FGATTManager.Instance.Callbacks.Show += Show;
        }

        protected override void InitializeModule()
        {
            Request();
        }

        protected void ATTAnswered(bool result)
        {
            FGATTManager.Instance.isATTCompliant = result;
            InitializationComplete(result);
        }


        public void Request()
        {
#if UNITY_IOS && !UNITY_EDITOR
            Version currentVersion = new Version(Device.systemVersion);
            Version iOSATT = new Version("14.5");
            Log("IOS device version : " + currentVersion + " target version : " + iOSATT);
            if (currentVersion >= iOSATT)
            {
                FGAnalytics.NewDesignEvent("ShowingATT:true");
                Log("Current platform : iOS 14.5+ - Showing ATT");
                Show();
            }
            else
            {
                Log("Current platform : iOS old version - No need for ATT");
                ATTAnswered(true);
            }
#else
            Log("Current platform : Android - No need for ATT");
            ATTAnswered(true);
#endif
        }

        public void Show()
        {
            ShowATT();
            Callbacks?._show?.Invoke();
        }

        protected override void ClearInitialization()
        {
            FGATTManager.Instance.Callbacks.Initialization -= Initialize;
            FGATTManager.Instance.Callbacks.Show -= Show;
        }
    }
}
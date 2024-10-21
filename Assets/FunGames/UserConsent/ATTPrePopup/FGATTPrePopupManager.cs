using FunGames.Core.Modules;
namespace FunGames.UserConsent.ATTPrePopup
{
    public class FGATTPrePopupManager : FGModuleAbstract<FGATTPrePopupManager,FGModuleCallbacks, FGATTPrePopupSettings>
    {
        public override FGATTPrePopupSettings Settings => FGATTPrePopupSettings.settings;
        protected override FGModule Parent => FGUserConsentManager.Instance;
        protected override string EventName => "ATTPrePopup";
        protected override string RemoteConfigKey => "FGATTPrePopup";

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

        protected override void ClearInitialization()
        {
            // FGRemoteConfig.Callbacks.OnInitialized -= _initialization;
        }
    }
}
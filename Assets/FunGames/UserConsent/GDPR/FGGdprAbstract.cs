using FunGames.Core;
using FunGames.Core.Modules;
using FunGames.Core.Settings;
using FunGames.RemoteConfig;

namespace FunGames.UserConsent.GDPR
{
    public abstract class FGGdprAbstract<M,S> : FGModuleAbstract<M, FGGdprCallbacks,S>
        where M : FGModuleAbstract<M, FGGdprCallbacks, S>
        where S : IFGModuleSettings
    {
        protected override FGModule Parent => FGGDPRManager.Instance;
        protected abstract int RemoteConfig { get; }
        protected abstract void RequestShow();
        protected abstract void UpdateAdditionalConsent();
        protected abstract void InitSdk();

        protected override void InitializeCallbacks()
        {
            InitWithoutTimer();
            FGGDPRManager.Instance.Callbacks.Initialization += Initialize;
            FGGDPRManager.Instance.Callbacks.Show += Show;
        }

        protected override void InitializeModule()
        {
            int gdprConfig = FGRemoteConfig.GetIntValue(FGGDPRManager.RC_GDPR_TYPE);
            if(!FunGamesSDK.IsConnectedToInternet) gdprConfig = 0;
            if (!RemoteConfig.Equals(gdprConfig))
            {
                LogWarning("GDPPR config is set to " + gdprConfig + " = No " + ModuleInfo.Name);
                ClearInitialization();
                InitializationComplete(true);
                return;
            }

            if (!FGUserConsentManager.Instance.IsGdprBeforeATT && !FGUserConsentManager.Instance.MustShowGdprIfAttRefused &&
                !FGUserConsent.IsAttCompliant)
            {
                Log("ATT refused : GDPR refused automatically");
                ClearInitialization();
                InitializationComplete(true);
                return;
            }

            InitSdk();
        }

        protected void Show()
        {
            RequestShow();
            Callbacks?._show?.Invoke();
        }

        protected void UpdateConsent(FGGDPRStatus status)
        {
            UpdateAdditionalConsent();
            FGUserConsent.GdprStatus.SetGDPRValues(status);
            Callbacks._consentUpdated?.Invoke(status);
            FGGDPRManager.Instance.Callbacks._consentUpdated?.Invoke(status);
        }

        protected override void ClearInitialization()
        {
            FGGDPRManager.Instance.Callbacks.Initialization -= Initialize;
            FGGDPRManager.Instance.Callbacks.Show -= Show;
        }
    }
}
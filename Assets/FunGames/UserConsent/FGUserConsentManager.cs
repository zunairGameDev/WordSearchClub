using System;
using FunGames.Core;
using FunGames.Core.Modules;
using FunGames.UserConsent.ATT;
using FunGames.UserConsent.ATTPrePopup;
using FunGames.UserConsent.GDPR;
using FunGames.RemoteConfig;

namespace FunGames.UserConsent
{
    public class
        FGUserConsentManager : FGModuleAbstract<FGUserConsentManager, FGModuleCallbacks, FGUserConsentSettings>
    {
        public const string RC_GDPR_BEFORE_ATT = "GdprBeforeAtt";
        public const string RC_GDPR_IF_ATT_REFUSED = "GdprIfAttRefused";

        private Action<bool> _initializePrepopupATT = delegate { FGATTPrePopupManager.Instance.Initialize(); };
        private Action<bool> _initializeATT = delegate { FGATTManager.Instance.Initialize(); };
        private Action<bool> _initializeGDPR = delegate { FGGDPRManager.Instance.Initialize(); };
        private Action<bool> _onComplete = delegate { FGUserConsent.TriggerCompleteCallback(); };
        private Action<bool> _onInitialize;
        private bool initialized = false;
        private bool _isGdprBeforeATT;
        private bool _mustShowGdprIfAttRefused;

        public bool IsGdprBeforeATT => _isGdprBeforeATT;
        public bool MustShowGdprIfAttRefused => _mustShowGdprIfAttRefused;

        public override FGUserConsentSettings Settings => FGUserConsentSettings.settings;
        protected override FGModule Parent => FGCore.Instance;
        protected override string EventName => "UserConsent";
        protected override string RemoteConfigKey => "FGUserConsent";

        protected override void InitializeCallbacks()
        {
            if (initialized) return;
            InitWithoutTimer();
            FGRemoteConfig.AddDefaultValue(RC_GDPR_BEFORE_ATT, 1);
            FGRemoteConfig.AddDefaultValue(RC_GDPR_IF_ATT_REFUSED, 0);

            _initializePrepopupATT = delegate { FGATTPrePopupManager.Instance.Initialize(); };
            _initializeATT = delegate { FGATTManager.Instance.Initialize(); };
            _initializeGDPR = delegate { FGGDPRManager.Instance.Initialize(); };
            _onInitialize += delegate(bool b) { Initialize(); };
            FGRemoteConfig.Callbacks.OnInitialized += _onInitialize;
            FGUserConsent.Callbacks.OnInitialized += _onComplete;
            initialized = true;
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
            _isGdprBeforeATT = FGRemoteConfig.GetBooleanValue(RC_GDPR_BEFORE_ATT);
            _mustShowGdprIfAttRefused = FGRemoteConfig.GetBooleanValue(RC_GDPR_IF_ATT_REFUSED);
            InitializationComplete(true);
            InitializeOrder();
        }

        protected override void ClearInitialization()
        {
            FGRemoteConfig.Callbacks.OnInitialized -= _onInitialize;
            FGUserConsent.Callbacks.OnInitialized -= _onComplete;

            FGGDPRManager.Instance.Callbacks.OnInitialized -= _initializePrepopupATT;
            FGATTPrePopupManager.Instance.Callbacks.OnInitialized -= _initializeATT;
            // FGATTManager.Instance.Callbacks.OnInitialized -= _onComplete;

            FGGDPRManager.Instance.Callbacks.OnInitialized -= _initializeATT;
            FGATTManager.Instance.Callbacks.OnInitialized -= _initializeGDPR;
            // FGGDPRManager.Instance.Callbacks.OnInitialized -= _onComplete;
        }

        private void InitializeOrder()
        {
            if (IsGdprBeforeATT)
            {
                FGGDPRManager.Instance.Callbacks.OnInitialized += _initializePrepopupATT;
                FGATTPrePopupManager.Instance.Callbacks.OnInitialized += _initializeATT;
                // FGATTManager.Instance.Callbacks.OnInitialized += _onComplete;
                FGGDPRManager.Instance.Initialize();
            }
            else
            {
                FGATTPrePopupManager.Instance.Callbacks.OnInitialized += _initializeATT;
                FGATTManager.Instance.Callbacks.OnInitialized += _initializeGDPR;
                // FGGDPRManager.Instance.Callbacks.OnInitialized += _onComplete;
                FGATTPrePopupManager.Instance.Initialize();
            }
        }
    }
}
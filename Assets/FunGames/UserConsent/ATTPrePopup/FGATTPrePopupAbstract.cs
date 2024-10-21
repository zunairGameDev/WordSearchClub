using FunGames.Core.Modules;
using FunGames.Core.Settings;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;

namespace FunGames.UserConsent.ATTPrePopup
{
    public abstract class FGATTPrePopupAbstract<M, S> : FGModuleAbstract<M, FGModuleCallbacks, S>
        where M : FGModuleAbstract<M, FGModuleCallbacks, S>
        where S : IFGModuleSettings

    {
        protected override FGModule Parent => FGATTPrePopupManager.Instance;
        protected abstract void Show();

        protected override void InitializeCallbacks()
        {
            InitWithoutTimer();
            FGATTPrePopupManager.Instance.Callbacks.Initialization += Initialize;
        }

        protected override void InitializeModule()
        {
#if UNITY_IOS
            if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                Show();
            }
            else
            {
                Log("ATT already responded - No need for ATT PrePopup");
                PrePopupAccepted();
            }
#else
            Log("Current platform : Android - No need for ATT PrePopup");
            PrePopupAccepted();
#endif
        }

        protected void PrePopupAccepted()
        {
            InitializationComplete(true);
        }

        protected override void ClearInitialization()
        {
            FGATTPrePopupManager.Instance.Callbacks.Initialization -= Initialize;
        }
    }
}
using System;
using FunGames.Core.Modules;

namespace FunGames.UserConsent.GDPR
{
    public class FGGdprCallbacks : FGModuleCallbacks
    {
        internal Action _show;
        internal Action<FGGDPRStatus> _consentUpdated;

        public event Action Show
        {
            add => _show += value;
            remove => _show -= value;
        }

        public event Action<FGGDPRStatus> OnConsentUpdated
        {
            add => _consentUpdated += value;
            remove => _consentUpdated -= value;
        }

        public override void Clear()
        {
            base.Clear();
            _show = null;
        }
    }
}
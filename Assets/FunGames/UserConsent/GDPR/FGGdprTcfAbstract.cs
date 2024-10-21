using FunGames.Core.Settings;
using UnityEngine;

namespace FunGames.UserConsent.GDPR
{
    public abstract class FGGdprTcfAbstract<M, S> : FGGdprAbstract<M, S>
        where M : FGGdprAbstract<M, S>
        where S : IFGModuleSettings
    {
        protected abstract string GetTcfString();

        protected override void UpdateAdditionalConsent()
        {
            string tcf = GetTcfString();
            FGGDPRManager.Instance.SetTCFString(tcf);
            Log("[TCF String] " + tcf);
        }
    }
}
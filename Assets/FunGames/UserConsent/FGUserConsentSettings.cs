using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGUserConsentSettings : FGModuleSettingsAbstract<FGUserConsentSettings>
    {
        public const string NAME = "FGUserConsentSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGUserConsentSettings LoadResources()
        {
            return Resources.Load<FGUserConsentSettings>(PATH);
        }
    }
}
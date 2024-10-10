using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent.GDPR.CustomGDPR
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGCustomGDPRSettings : FGModuleSettingsAbstract<FGCustomGDPRSettings>
    {
        public const string NAME = "FGCustomGDPRSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGCustomGDPRSettings LoadResources()
        {
            return Resources.Load<FGCustomGDPRSettings>(PATH);
        }
    }
}
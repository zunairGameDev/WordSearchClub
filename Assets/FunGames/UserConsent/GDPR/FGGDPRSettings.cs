using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGGDPRSettings : FGModuleSettingsAbstract<FGGDPRSettings>
    {
        public const string NAME = "FGGDPRSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGGDPRSettings LoadResources()
        {
            return Resources.Load<FGGDPRSettings>(PATH);
        }
    }
}
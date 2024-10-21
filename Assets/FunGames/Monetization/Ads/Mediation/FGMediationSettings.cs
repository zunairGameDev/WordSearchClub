using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.Mediation
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGMediationSettings : FGModuleSettingsAbstract<FGMediationSettings>
    {
        public const string NAME = "FGMediationSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGMediationSettings LoadResources()
        {
            return Resources.Load<FGMediationSettings>(PATH);
        }
    }
}
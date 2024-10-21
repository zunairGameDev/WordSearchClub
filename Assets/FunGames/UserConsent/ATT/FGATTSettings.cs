using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGATTSettings : FGModuleSettingsAbstract<FGATTSettings>
    {
        public const string NAME = "FGATTSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGATTSettings LoadResources()
        {
            return Resources.Load<FGATTSettings>(PATH);
        }
    }
}
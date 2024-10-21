using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGATTPrePopupSettings : FGModuleSettingsAbstract<FGATTPrePopupSettings>
    {
        public const string NAME = "FGATTPrePopupSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGATTPrePopupSettings LoadResources()
        {
            return Resources.Load<FGATTPrePopupSettings>(PATH);
        }
    }
}
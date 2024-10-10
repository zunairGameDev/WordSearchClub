using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent.ATT.UnityATT
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGUnityATTSettings : FGModuleSettingsAbstract<FGUnityATTSettings>
    {
        public const string NAME = "FGUnityATTSettings";
        private const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGUnityATTSettings LoadResources()
        {
            return Resources.Load<FGUnityATTSettings>(PATH);
        }
    }
}
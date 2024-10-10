using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.Analytics.FirebaseA
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGFirebaseSettings : FGModuleSettingsAbstract<FGFirebaseSettings>
    {
        public const string NAME = "FGFirebaseSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGFirebaseSettings LoadResources()
        {
            return Resources.Load<FGFirebaseSettings>(PATH);
        }
    }
}
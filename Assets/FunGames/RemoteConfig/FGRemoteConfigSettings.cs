using System.Collections.Generic;
using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.RemoteConfig
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGRemoteConfigSettings : FGModuleSettingsAbstract<FGRemoteConfigSettings>
    {
        public const string NAME = "FGRemoteConfigSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        private List<FGRemoteConfigValue> _defaultValues = null;

        protected override FGRemoteConfigSettings LoadResources()
        {
            return Resources.Load<FGRemoteConfigSettings>(PATH);
        }

        public List<FGRemoteConfigValue> CustomDefaultValues = new List<FGRemoteConfigValue>();
    }
}
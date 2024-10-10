using System;
using FunGames.Core.Utils;
using UnityEngine;

namespace FunGames.UserConsent.GDPR.GoogleUMP
{
    [CreateAssetMenu(fileName = FGPath.ASSETS_RESOURCES + PATH, menuName = PATH, order = ORDER)]
    public class FGGoogleUMPSettings : FGModuleSettingsAbstract<FGGoogleUMPSettings>
    {
        public const string NAME = "FGGoogleUMPSettings";
        const string PATH = FGPath.FUNGAMES + "/" + NAME;

        protected override FGGoogleUMPSettings LoadResources()
        {
            return Resources.Load<FGGoogleUMPSettings>(PATH);
        }
        
        [Header("Test mode")] public bool TestMode = false;
        public string TestDeviceID = String.Empty;
    }
}
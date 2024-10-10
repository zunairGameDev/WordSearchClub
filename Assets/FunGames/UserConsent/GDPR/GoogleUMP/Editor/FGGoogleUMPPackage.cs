using System.Collections.Generic;
using FunGames.Core.Editor;
using FunGames.Tools.Utils;
using UnityEditor;

namespace FunGames.UserConsent.GDPR.GoogleUMP.Editor
{
    public class FGGoogleUMPPackage : FGPackageAbstract<FGGoogleUMPPackage>
    {
        public override string JsonName => "fg_google_cmp.json";
        public override string PackageName => "FGGoogleUMP";
        public override string ModuleFolder => "UserConsent/GDPR/GoogleUMP";
        public override string SettingsAssetName => FGGoogleUMPSettings.NAME;
        public override string DeploymentFolder => "UserConsent/GDPR/fg_google_cmp";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Recurse;
        
        // protected override string[] assetFolders()
        // {
        //     List<string> folders = new List<string>();
        //     folders.AddRange(base.assetFolders());
        //     folders.Add("Assets/Editor/UniWebView");
        //     return folders.ToArray();
        // }
        
        protected override string[] externalAssets()
        {
            List<string> externalAssets = new List<string>();
            externalAssets.AddRange(AssetsUtils.GetAssetsPath(FUNGAMES_EXTERNALS_PATH, "GoogleMobileAds"));
            string[] gameAnalyticsFolder = {"Assets/GameAnalytics/Plugins/Scripts/ILRD/AdMob"};
            externalAssets.AddRange(AssetsUtils.GetAssetsPath(gameAnalyticsFolder, "GAAdMobIntegration"));
            return externalAssets.ToArray();
        }
        
        public override void AddPrefabs()
        {
            FGGDPRPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
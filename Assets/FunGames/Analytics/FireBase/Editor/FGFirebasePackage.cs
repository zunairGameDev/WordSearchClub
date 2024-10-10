using System.Collections.Generic;
using FunGames.Core.Editor;
using FunGames.Tools.Utils;
using UnityEditor;

namespace FunGames.Analytics.FirebaseA
{
    public class FGFirebasePackage : FGPackageAbstract<FGFirebasePackage>
    {
        public override string JsonName => "fg_firebase.json";
        public override string PackageName => "FGFirebase";
        public override string ModuleFolder => "Analytics/Firebase";
        public override string SettingsAssetName => FGFirebaseSettings.NAME;
        public override string DeploymentFolder => "Analytics/fg_firebase";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Recurse;

        public override void AddPrefabs()
        {
            FGAnalyticsPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }

        protected override string[] externalAssets()
        {
            List<string> externalAssets = new List<string>();
            externalAssets.AddRange(AssetsUtils.GetAssetsPath(FUNGAMES_EXTERNALS_PATH, "FirebaseAnalytics"));
            externalAssets.AddRange(AssetsUtils.GetAssetsPath(FUNGAMES_EXTERNALS_PATH, "FirebaseCrashlytics"));
            return externalAssets.ToArray();
        }
    }
}
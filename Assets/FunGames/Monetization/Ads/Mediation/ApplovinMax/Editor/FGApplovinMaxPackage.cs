using FunGames.Core.Editor;
using FunGames.Tools.Utils;
using UnityEditor;

namespace FunGames.Mediation.ApplovinMax
{
    public class FGApplovinMaxPackage : FGPackageAbstract<FGApplovinMaxPackage>
    {
        public override string JsonName => "fg_applovin_max.json";
        public override string PackageName => "FGApplovinMax";
        public override string ModuleFolder => "Monetization/Ads/Mediation/ApplovinMax";
        public override string SettingsAssetName => FGApplovinMaxSettings.NAME;
        public override string DeploymentFolder => "Monetization/Ads/Mediation/fg_applovin_max";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Recurse;
        protected override string[] externalAssets()
        {
            return AssetsUtils.GetAssetsPath(FUNGAMES_EXTERNALS_PATH, "Applovin").ToArray();
        }
        
        public override void AddPrefabs()
        {
            FGMediationPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
using FunGames.Core.Editor;
using UnityEditor;

namespace FunGames.UserConsent.GDPR.CustomGDPR.Editor
{
    public class FGCustomGdprPackage : FGPackageAbstract<FGCustomGdprPackage>
    {
        public override string JsonName => "fg_custom_gdpr.json";
        public override string PackageName => "FGCustomGdpr";
        public override string ModuleFolder => "UserConsent/GDPR/CustomGDPR";
        public override string SettingsAssetName => FGCustomGDPRSettings.NAME;
        public override string DeploymentFolder => "UserConsent/GDPR/fg_custom_gdpr";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Recurse;
        
        public override void AddPrefabs()
        {
            FGGDPRPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
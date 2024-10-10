using FunGames.Core.Editor;
using FunGames.UserConsent.Editor;
using UnityEditor;

namespace FunGames.UserConsent.GDPR
{
    public class FGGDPRPackage : FGPackageAbstract<FGGDPRPackage>
    {
        public override string JsonName => "fg_gdpr.json";
        public override string PackageName => "FGGDPR";
        public override string ModuleFolder => "UserConsent/GDPR";
        public override string SettingsAssetName => FGGDPRSettings.NAME;
        public override string DeploymentFolder => "UserConsent/GDPR/fg_gdpr";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
        
        public override void AddPrefabs()
        {
            FGUserConsentPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
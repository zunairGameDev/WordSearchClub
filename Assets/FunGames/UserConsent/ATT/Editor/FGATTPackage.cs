using FunGames.Core.Editor;
using FunGames.UserConsent.Editor;
using UnityEditor;

namespace FunGames.UserConsent.GDPR
{
    public class FGATTPackage : FGPackageAbstract<FGATTPackage>
    {
        public override string JsonName => "fg_att.json";
        public override string PackageName => "FGATT";
        public override string ModuleFolder => "UserConsent/ATT";
        public override string SettingsAssetName => FGATTSettings.NAME;
        public override string DeploymentFolder => "UserConsent/ATT/fg_att";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
        
        public override void AddPrefabs()
        {
            FGUserConsentPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
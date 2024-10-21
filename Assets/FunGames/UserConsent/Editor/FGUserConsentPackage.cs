using FunGames.Core.Editor;
using UnityEditor;

namespace FunGames.UserConsent.Editor
{
    public class FGUserConsentPackage:FGPackageAbstract<FGUserConsentPackage>

    {
        public override string JsonName => "fg_user_consent.json";
        public override string PackageName => "FGUserConsent";
        public override string ModuleFolder => "UserConsent";
        public override string SettingsAssetName => FGUserConsentSettings.NAME;
        public override string DeploymentFolder => "UserConsent/fg_user_consent";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
    }
}
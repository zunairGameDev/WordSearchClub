using FunGames.Core.Editor;
using FunGames.UserConsent.GDPR;
using UnityEditor;

namespace FunGames.UserConsent.ATT.UnityATT
{
    public class FGUnityATTPackage : FGPackageAbstract<FGUnityATTPackage>
    {
        public override string JsonName => "fg_unity_att.json";
        public override string PackageName => "FGUnityATT";
        public override string ModuleFolder => "UserConsent/ATT/UnityATT";
        public override string SettingsAssetName => FGUnityATTSettings.NAME;
        public override string DeploymentFolder => "UserConsent/ATT/fg_unity_att";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Recurse;
        
        public override void AddPrefabs()
        {
            FGATTPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }
}
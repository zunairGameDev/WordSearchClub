using FunGames.Core.Editor;
using UnityEditor;

namespace FunGames.RemoteConfig
{
    public class FGRemoteConfigPackage : FGPackageAbstract<FGRemoteConfigPackage>
    {
        public override string JsonName => "fg_remote_config.json";
        public override string ModuleFolder => "RemoteConfig";
        public override string PackageName => "FGRemoteConfig";
        public override string SettingsAssetName => FGRemoteConfigSettings.NAME;
        public override string DeploymentFolder => "RemoteConfig/fg_remote_config";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
    }
}
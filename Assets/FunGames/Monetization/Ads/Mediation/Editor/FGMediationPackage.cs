using FunGames.Core.Editor;
using UnityEditor;

namespace FunGames.Mediation
{
    public class FGMediationPackage : FGPackageAbstract<FGMediationPackage>
    {
        public override string JsonName => "fg_mediation.json";
        public override string PackageName => "FGMediation";
        public override string ModuleFolder => "Monetization/Ads/Mediation";
        public override string SettingsAssetName => FGMediationSettings.NAME;
        public override string DeploymentFolder => "Monetization/Ads/Mediation/fg_mediation";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
    }
}
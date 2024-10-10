using FunGames.Core.Editor;
using FunGames.UserConsent.Editor;
using UnityEditor;

namespace FunGames.UserConsent.GDPR
{
    public class FGATTPrePopupPackage : FGPackageAbstract<FGATTPrePopupPackage>
    {
        public override string JsonName => "fg_att_pre_popup.json";
        public override string PackageName => "FGATTPrePopup";
        public override string ModuleFolder => "UserConsent/ATTPrePopup";
        public override string SettingsAssetName => FGATTPrePopupSettings.NAME;
        public override string DeploymentFolder => "UserConsent/ATTPrePopup/fg_att_pre_popup";
        public override ExportPackageOptions ExportOptions => ExportPackageOptions.Default;
        
        public override void AddPrefabs()
        {
            FGUserConsentPackage.Package.AddPrefabs();
            base.AddPrefabs();
        }
    }   
}
using FunGames.Core.Modules;
using UnityEditor;
using UnityEngine;

namespace FunGames.Core.Settings
{
    public abstract class FGModuleSettings : ScriptableObject, IFGModuleSettings
    {
        [HideInInspector] public FGModuleInfo moduleInfo;

        public Color LogColor = Color.white;
        public Color Color => LogColor;
        
        public bool logEnabled = true;
        public bool LogEnabled => logEnabled;

        public FGModuleInfo ModuleInfo
        {
            get => moduleInfo;
            set
            {
                moduleInfo = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
#endif
            }
        }
    }
}
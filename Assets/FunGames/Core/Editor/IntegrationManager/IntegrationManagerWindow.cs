using System;
using System.Collections.Generic;
using System.Text;
using FunGames.Core.Editor.Config;
using FunGames.Tools.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FunGames.Core.Editor.IntegrationManager
{
    public class IntegrationManagerWindow : EditorWindow
    {
        public GUISkin MainSkin;
        private Vector2 scrollPos = Vector2.zero;
        private float previousWindowWidth;
        private float w1;
        private float w2;
        private float w3;
        private float w4;
        private float w5;
        private float w6;
        private const float SPACING = 10;

        private static bool isInit = false;
        private static bool isViewUpdated = false;

        private static IntegrationManagerController _imc;
        List<FGModuleInstallDrawer> _moduleInstallDrawers = new List<FGModuleInstallDrawer>();

        [MenuItem("FunGames/Integration Manager")]
        public static void Init()
        {
            GetWindow<IntegrationManagerWindow>();
        }

        void OnEnable()
        {
            titleContent.text = "FunGames SDK ";
            position = new Rect(200, 200, 740, 600); //w820
            minSize = new Vector2(position.width, 190);
            maxSize = new Vector2(position.width, position.height);
            previousWindowWidth = maxSize.x;
            isInit = false;

            w1 = 155; //180; //position.width * 0.2f;
            w2 = 80; //120; //position.width * 0.2f;
            w3 = 80; //120; //position.width * 0.2f;
            w4 = 100; //position.width * 0.4f / 3;
            w5 = 100; //position.width * 0.4f / 2;
            w6 = 100; //position.width * 0.4f / 3;

            _imc = new IntegrationManagerController();
            _imc.OnDataLoaded += () => isInit = true;
            _imc.Initialize();
            
            _moduleInstallDrawers.Clear();
        }

        void OnGUI()
        {
            if (Math.Abs(previousWindowWidth - position.width) > 1)
            {
                previousWindowWidth = position.width;
            }

            GUI.skin = MainSkin;

            DrawHeader();

            if (!isInit)
            {
                GUILayout.Label("Loading...", EditorStyles.boldLabel, GUILayout.Width(w1));
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(this.position.width));
            DrawAllModules();
            GUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.Space(SPACING);
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Module Name", EditorStyles.boldLabel, GUILayout.Width(w1));
            GUILayout.Space(23);
            GUILayout.Label("Current", EditorStyles.boldLabel, GUILayout.Width(w2));
            GUILayout.Label("Latest", EditorStyles.boldLabel, GUILayout.Width(w3));
            GUILayout.Space(w4 + w5 + w6 );
            // GUILayout.Label("", EditorStyles.boldLabel, GUILayout.Width(w4));
            // GUILayout.Label("", EditorStyles.boldLabel, GUILayout.Width(w5));
            DrawPrefabButton();
            DrawGenerateConfigFileButton();
            GUILayout.EndHorizontal();
        }

        private void DrawAllModules()
        {
            // foreach (var moduleList in _imc.Data.MainJson.Versions)
            // {
            //     FGModuleInstallDrawer drawer = new FGModuleInstallDrawer(_imc, this);
            //     drawer.Draw(moduleList, 0);
            // }

            for (int i = 0; i < _imc.Data.MainJson.Versions.Count; i++)
            {
                if(_moduleInstallDrawers.Count <= i) _moduleInstallDrawers.Add(new FGModuleInstallDrawer(_imc, this)); 
                FGModuleInstallDrawer drawer = _moduleInstallDrawers[i];
                drawer.Draw(_imc.Data.MainJson.Versions[i], i);
            }
        }

        private void DrawPrefabButton()
        {
            var iconPath = FGModuleInstallDrawer.ICONS_PATH + "Box-Icon.png";
            Texture icon = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture)) as Texture;
            string tooltip = "Add All Prefab(s) to Scene and Create Settings assets";
            GUILayoutOption[] options = { GUILayout.Width(32), GUILayout.Height(20) };
            if (GUILayout.Button(new GUIContent(icon, tooltip), options))
            {
                // Check if there is already an EventSystem in the scene
                if (FindObjectOfType<EventSystem>() == null)
                {
                    // Create a new EventSystem
                    GameObject eventSystemObject = new GameObject("EventSystem");
                    eventSystemObject.AddComponent<EventSystem>();
                    eventSystemObject
                        .AddComponent<
                            StandaloneInputModule>(); // Optional: Add an input module (e.g., for mouse/keyboard input)
                }

                FGPackage[] allPackages = ProjectUtils.GetEnumerableOfType<FGPackage>().ToArray();
                foreach (var package in allPackages)
                {
                    package.AddPrefabs();
                    package.CreateSettingsAsset();
                }
            }
        }

        private void DrawGenerateConfigFileButton()
        {
            var iconPath = FGModuleInstallDrawer.ICONS_PATH + "Share-Icon_2.png";
            Texture icon = AssetDatabase.LoadAssetAtPath(iconPath, typeof(Texture)) as Texture;
            string tooltip = "Export Config File";
            GUILayoutOption[] options = { GUILayout.Width(32), GUILayout.Height(20) };
            if (GUILayout.Button(new GUIContent(icon, tooltip), options))
            {
                FGConfigFile.Export();
            }
        }

    }

    public class TooltipBuilder
    {
        List<string> tooltips = new List<string>();

        public void Add(string tooltip)
        {
            tooltips.Add(tooltip);
        }

        public override string ToString()
        {
            StringBuilder tooltipBuilder = new StringBuilder();
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips.Count > 1) tooltipBuilder.Append(i + ". ");
                tooltipBuilder.Append(tooltips[i]);
                if (tooltips.Count > 1 && i != tooltips.Count - 1) tooltipBuilder.Append("\n");
                if (i != tooltips.Count - 1) tooltipBuilder.Append("\n");
            }

            return tooltipBuilder.ToString();
        }
    }
}
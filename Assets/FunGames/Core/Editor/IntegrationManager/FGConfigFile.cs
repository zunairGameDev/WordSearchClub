using System;
using System.IO;
using System.Text;
using FunGames.Tools.Utils;
using UnityEditor;
using UnityEngine;

namespace FunGames.Core.Editor.Config
{
    public class FGConfigFile
    {
        // [MenuItem("FunGames/Generate Config File")]
        public static void Export()
        {
            string defaultName = "fg_config (" + Application.productName + "-" + Application.version + ")";
            string filePath = EditorUtility.SaveFilePanel("Export FG Config", "", defaultName,"csv");
            if (String.IsNullOrEmpty(filePath)) return;
            File.WriteAllText(filePath, BuildConfigText());
        }

        private static string BuildConfigText()
        {
            string separator = ",";
            StringBuilder sb = new StringBuilder();
            foreach (FGPackage package in ProjectUtils.GetEnumerableOfType<FGPackage>())
            {
                sb.Append(package.ModuleInfo.Id + separator + package.ModuleInfo.Version + "\n");
            }
            return sb.ToString();
        }
    }
}
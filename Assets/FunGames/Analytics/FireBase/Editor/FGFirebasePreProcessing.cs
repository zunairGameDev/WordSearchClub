using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FunGames.Analytics.FirebaseA
{
    public class FGFirebasePreProcessing : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
#if UNITY_ANDROID
            string manifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

            if (!File.Exists(manifestPath))
            {
                Debug.LogError("AndroidManifest.xml not found at path: " + manifestPath);
                return;
            }

            string manifestContent = File.ReadAllText(manifestPath);

            // Add the comment at the end of the file
            string commentToAdd = "<!-- AnalyticsFixPropertyRemover -->";

            if (manifestContent.Contains(commentToAdd)) return;
            manifestContent += "\n" + commentToAdd;

            File.WriteAllText(manifestPath, manifestContent);
            Debug.Log("AndroidManifest.xml modified successfully.");
#endif
        }
    }
}
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using FunGames.Core.Editor;
using FunGames.Tools.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class FGPostProcessing : IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get { return 1; }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        SetUpAndroidPermissions(report.summary.platform, string.Empty);
        UpdateAllSettings();
    }

    public static void SetUpAndroidPermissions(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.Android)
        {
#if UNITY_ANDROID
            AndroidManifestParser.Instance.AddAllPermissions();
#endif
        }
    }

#if UNITY_IOS
    // [PostProcessBuild]
    // public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    // {
    //     if (buildTarget == BuildTarget.iOS)
    //     {
    //         string plistPath = path + "/Info.plist";
    //         PlistDocument plist = new PlistDocument();
    //         plist.ReadFromFile(plistPath);
    //
    //         PlistElementDict rootDict = plist.root;
    //
    //         if (rootDict["NSAppTransportSecurity"] == null)
    //         {
    //             rootDict.CreateDict("NSAppTransportSecurity");
    //         }
    //
    //         rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
    //         rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoadsInWebContent", true);
    //
    //         var exceptionDomains = rootDict["NSAppTransportSecurity"].AsDict().CreateDict("NSExceptionDomains");
    //         var domain = exceptionDomains.CreateDict("ip-api.com");
    //
    //         domain.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
    //         domain.SetBoolean("NSIncludesSubdomains", true);
    //
    //         File.WriteAllText(plistPath, plist.WriteToString());
    //     }
    // }
#endif

    private void UpdateAllSettings()
    {
        List<FGPackage> packages = ProjectUtils.GetEnumerableOfType<FGPackage>();
        foreach (FGPackage package in packages)
        {
            package.UpdateSettings();
        }
    }
}
#endif
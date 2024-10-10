using GameAnalyticsSDK;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace FunGames.Analytics.GA
{
    public class FGGameAnalyticsPreProcessing : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1000;

        public void OnPreprocessBuild(BuildReport report)
        {
            GameAnalytics.SettingsGA.InfoLogBuild = false;
            GameAnalytics.SettingsGA.InfoLogEditor = false;
            GameAnalytics.SettingsGA.SubmitFpsAverage = true;
            GameAnalytics.SettingsGA.SubmitFpsCritical = true;
            AddCustomDimention01(FGGameAnalytics.HIGH_END);
            AddCustomDimention01(FGGameAnalytics.LOW_END);
        }

        private void AddCustomDimention01(string dimension)
        {
            if(GameAnalytics.SettingsGA.CustomDimensions01.Contains(dimension)) return;
            GameAnalytics.SettingsGA.CustomDimensions01.Add(dimension);
        }
    }
}
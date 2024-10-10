using System;
using System.Collections.Generic;
using System.Globalization;
using FunGames.Core.Modules;
using UnityEngine;

namespace FunGames.RemoteConfig
{
    public static class FGRemoteConfig
    {
        public static FGModuleCallbacks Callbacks => FGRemoteConfigManager.Instance.Callbacks;

        public static FGABTest CurrentABTest => FGRemoteConfigManager.Instance.CurrentABTest;

        /// <summary>
        /// Must be called on Awake !
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddDefaultValue(string key, object value) =>
            FGRemoteConfigManager.Instance.AddDefaultValue(key, value);

        /// <summary>
        /// Must be called on Awake !
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void OverrideDefaultValue(string key, object value) =>
            FGRemoteConfigManager.Instance.OverrideDefaultValue(key, value);

        /// <summary>
        /// Must be called on Awake !
        /// </summary>
        /// <param name="defaultValues"></param>
        public static void AddDefaultValues(Dictionary<string, object> defaultValues) =>
            FGRemoteConfigManager.Instance.AddDefaultValues(defaultValues);

        public static int GetIntValue(string key)
        {
            try
            {
                return Convert.ToInt32(GetValueByKey(key));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }

        public static double GetDoubleValue(string key)
        {
            try
            {
                string value = GetValueByKey(key).ToString();
                value = value.Replace(" ", String.Empty);
                if (!value.Contains(".")) value = value.Replace(",", ".");
                return Convert.ToDouble(value, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }

        public static float GetFloat(string key)
        {
            try
            {
                string value = GetValueByKey(key).ToString();
                value = value.Replace(NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, ".");
                value = value.Replace(NumberFormatInfo.CurrentInfo.NumberGroupSeparator, ",");
                return float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return -1;
            }
        }

        public static bool GetBooleanValue(string key)
        {
            try
            {
                return Convert.ToBoolean(GetValueByKey(key));
            }
            catch (Exception e)
            {
                string val = GetValueByKey(key).ToString();
                if (val == "0" || val == "false") return false;
                if (val == "1" || val == "true") return true;
                
                Debug.LogError(e);
                return false;
            }
        }

        public static string GetStringValue(string key)
        {
            return GetValueByKey(key).ToString();
        }

        private static object GetValueByKey(string key)
        {
            return FGRemoteConfigManager.Instance.GetValueByKey(key);
        }
    }
}
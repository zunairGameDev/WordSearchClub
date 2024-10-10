using System;
using FunGames.Tools.Utils;
using UnityEngine;

namespace FunGames.UserConsent.GDPR
{
    public class FGCMPReader
    {
        public const string TCF_STRING = "IABTCF_TCString";
        public const string VENDOR_CONSENTS = "IABTCF_VendorConsents";
        public const string VENDOR_INTERESTS = "IABTCF_VendorLegitimateInterests";
        public const string PURPOSE_CONSENTS = "IABTCF_PurposeConsents";
        public const string PURPOSE_INTERESTS = "IABTCF_PurposeLegitimateInterests";
        public const string PUBLISHER_CONSENT = "IABTCF_PublisherConsent";
        public const string PUBLISHER_INTERESTS = "IABTCF_PublisherLegitimateInterests";

        public static string GetTCFString()
        {
            GetVendorConsents();
            GetVendorInterests();
            GetPurposeConsents();
            GetPurposeInterests();
            GetPublisherConsent();
            GetPublisherInterests();
            return GetString(TCF_STRING);
        }

        public static bool HasGenericConsent()
        {
            return GetPurposeConsents().StartsWith("1111111111");
        }

        public static string GetVendorConsents()
        {
            return GetString(VENDOR_CONSENTS);
        }

        public static string GetVendorInterests()
        {
            return GetString(VENDOR_INTERESTS);
        }

        public static string GetPurposeConsents()
        {
            return GetString(PURPOSE_CONSENTS);
        }

        public static string GetPurposeInterests()
        {
            return GetString(PURPOSE_INTERESTS);
        }

        public static string GetPublisherConsent()
        {
            return GetString(PUBLISHER_CONSENT);
        }

        public static string GetPublisherInterests()
        {
            return GetString(PUBLISHER_INTERESTS);
        }

        public static string GetString(string key)
        {
            string str = String.Empty;

            if (CurrentPlatform.Is(Platform.Android))
            {
                str = AndroidSharedPreferences.GetString(key);
            }
            else if (CurrentPlatform.Is(Platform.IOS) && PlayerPrefs.HasKey(key))
            {
                str = PlayerPrefs.GetString(key);
            }

            FGGDPRManager.Instance.Log("[FGCMPReader] " + key + ": " + str);
            return str;
        }
    }
}
using System;
using System.Collections.Generic;
using FunGames.RemoteConfig;
using FunGames.Tools.Utils;

namespace FunGames.UserConsent.GDPR.CustomGDPR
{
    public abstract class FGCustomGDPRAbstract<M> : FGGdprAbstract<M, FGCustomGDPRSettings>
        where M : FGGdprAbstract<M, FGCustomGDPRSettings>
    {
        public List<FGGDPRDisplayAbstract> gdprWindows = new List<FGGDPRDisplayAbstract>();
        private FGGDPRDisplayAbstract _currentGdpr;

        protected override void InitSdk()
        {
            Log("Fetching localization...");
            LocalisationUtils.GetLocalisationCode(InitWithLocalisation);
        }

        protected override void RequestShow()
        {
            int gdprDisplay = FGRemoteConfig.GetIntValue(FGGDPRManager.RC_GDPR_DISPLAY);
            foreach (var gdpr in gdprWindows)
            {
                if (gdpr.RemoteConfigValue.Equals(gdprDisplay))
                {
                    _currentGdpr = Instantiate(gdpr, transform);
                    break;
                }
            }

            _currentGdpr.OnValidated += (s) =>
            {
                UpdateConsent(s);
                InitializationComplete(true);
            };

            _currentGdpr.ShowGDPR();
        }

        protected override void UpdateAdditionalConsent()
        {
            // Nothing to do here
        }

        private void InitWithLocalisation(Location location)
        {
            if (String.IsNullOrEmpty(location.countryCode)) Log("Couldn't retrieve localisation code !");
            else Log("Localisation found : {" + location.countryCode + " ; " + location.regionCode + "}");

            FGGDPRManager.Instance.location = location;

            if (FGGDPRManager.Instance.IsGDPRAlreadyAnswered &&
                FGGDPRManager.Instance.GdprStatus.IsFullyAccepted)
            {
                Log("GDPR already responded - Accepted ");
                UpdateConsent(FGGDPRManager.Instance.GdprStatus);
                InitializationComplete(true);
            }
            else if (LocalisationUtils.isGDPRApplied(location))
            {
                Show();
            }
            else
            {
                Log("No need for GDPR");
                UpdateConsent(FGGDPRStatus.FullyAccepted);
                InitializationComplete(true);
            }
        }
    }
}
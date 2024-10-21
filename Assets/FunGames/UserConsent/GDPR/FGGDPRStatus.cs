namespace FunGames.UserConsent.GDPR
{
    public class FGGDPRStatus
    {
        public bool TargetedAdvertisingAccepted = true;
        public bool AnalyticsAccepted = true;
        public bool IsAboveAgeLimit = true;

        public void SetGDPRValues(bool result)
        {
            TargetedAdvertisingAccepted = result;
            IsAboveAgeLimit = result;
        }
        
        public void SetGDPRValues(FGGDPRStatus fgGdprStatus)
        {
            TargetedAdvertisingAccepted = fgGdprStatus.TargetedAdvertisingAccepted;
            IsAboveAgeLimit = fgGdprStatus.IsAboveAgeLimit;
            AnalyticsAccepted = fgGdprStatus.AnalyticsAccepted;
        }

        public bool IsFullyAccepted
        {
            get => TargetedAdvertisingAccepted && IsAboveAgeLimit;
        }

        public static FGGDPRStatus FullyAccepted
        {
            get
            {
                FGGDPRStatus status = new FGGDPRStatus();
                status.SetGDPRValues(true);
                return status;
            }
        }
        
        public static FGGDPRStatus Refused
        {
            get
            {
                FGGDPRStatus status = new FGGDPRStatus();
                status.SetGDPRValues(false);
                return status;
            }
        }
    }
}
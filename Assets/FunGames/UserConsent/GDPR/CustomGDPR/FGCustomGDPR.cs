namespace FunGames.UserConsent.GDPR.CustomGDPR
{
    public class FGCustomGDPR : FGCustomGDPRAbstract<FGCustomGDPR>
    {
        public override FGCustomGDPRSettings Settings => FGCustomGDPRSettings.settings;
        protected override string EventName => "TN CUSTOM GDPR";
        protected override string RemoteConfigKey => "FGCustomGDPR";

        protected override void OnAwake()
        {
            // throw new System.NotImplementedException();
        }

        protected override void OnStart()
        {
            // throw new System.NotImplementedException();
        }

        protected override int RemoteConfig => 0;
    }
}
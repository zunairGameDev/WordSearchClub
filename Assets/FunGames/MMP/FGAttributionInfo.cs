namespace FunGames.MMP
{
    public class FGAttributionInfo
    {
        public string network { get; private set; }
        public string adgroup { get; private set; }
        public string campaign { get; private set; }
        public string creative { get; private set; }
        public string trackerName { get; private set; }
        public string trackerToken { get; private set; }

        private FGAttributionInfo()
        {
        }

        public class Builder
        {
            private FGAttributionInfo _attributionInfo;

            public Builder()
            {
                _attributionInfo = new FGAttributionInfo();
            }

            public Builder SetNetwork(string network)
            {
                _attributionInfo.network = network;
                return this;
            }

            public Builder SetAdGroup(string adGroup)
            {
                _attributionInfo.adgroup = adGroup;
                return this;
            }
            
            public Builder SetCampaign(string campaign)
            {
                _attributionInfo.campaign = campaign;
                return this;
            }
            
            public Builder SetCreative(string creative)
            {
                _attributionInfo.creative = creative;
                return this;
            }
            
            public Builder SetTrackerName(string trackerName)
            {
                _attributionInfo.trackerName = trackerName;
                return this;
            }
            
            public Builder SetTrackerToken(string trackerToken)
            {
                _attributionInfo.trackerToken = trackerToken;
                return this;
            }
            
            public FGAttributionInfo Build()
            {
                return _attributionInfo;
            }
        }
    }
}
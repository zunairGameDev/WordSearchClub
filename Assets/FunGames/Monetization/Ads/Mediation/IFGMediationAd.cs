using System;
using FunGames.Core;

namespace FunGames.Mediation
{
    public interface IFGMediationAd
    {
        public string AdUnitId { get; set; }
        public FGAdType adType { get; }
        public void InitializeCallbacks();

        public void Load();

        public void Show(string placementName="default", Action<bool> callback=null);

        public void Hide();
            
        public bool IsReady();
        
        public bool IsShowing();
    }
}
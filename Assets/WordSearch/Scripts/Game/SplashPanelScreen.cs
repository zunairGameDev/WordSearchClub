using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.WordSearch
{
    public class SplashPanelScreen : Screen
    {
        public override void Show(bool back, bool immediate)
        {
            base.Show(back, immediate);

            

#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased += IAPProductPurchased;
#endif
        }
        public override void Hide(bool back, bool immediate)
        {
            base.Hide(back, immediate);

#if BBG_MT_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased -= IAPProductPurchased;
#endif
        }

    }
}

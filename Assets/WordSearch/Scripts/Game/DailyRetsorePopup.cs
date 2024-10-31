using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.WordSearch
{
    public class DailyRetsorePopup : Popup
    {
        public RestorePanel restorePanel;
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);

            restorePanel.ApplyData();
        }
    }
}


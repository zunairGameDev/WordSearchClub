using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace BBG.WordSearch
{
    public class DailyLevelComplete : Popup
    {
        public override void OnShowing(object[] inData)
        {
            base.OnShowing(inData);
            GetComponent<DailyChallengeWinPopup>().ApplyingData();
            //winPanel.ToShowData();
            //bool progressLevelCompleted = (bool)inData[0];
            //int coinsAwarded = (int)inData[1];
            //int keyAwarded = (int)inData[2];
            //bool lastLevel = (bool)inData[3];

            //playAgainButton.SetActive(!progressLevelCompleted);
            //nextLevelButton.SetActive(/*progressLevelCompleted && !lastLevel*/true);

            //bool awardCoins = coinsAwarded > 0;
            //bool awardKeys = keyAwarded > 0;

            //rewardsContainer.SetActive(awardCoins || awardKeys);
            //coinRewardContainer.SetActive(awardCoins);
            //keyRewardContainer.SetActive(awardKeys);

            //coinRewardAmountText.text = "x " + coinsAwarded;
            //keyRewardAmountText.text = "x " + keyAwarded;
        }
    }
}


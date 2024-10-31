using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DailyChallengeWinPopup : MonoBehaviour
{
    public TextMeshProUGUI dayValue;
    public TextMeshProUGUI dayText;
    public Transform rotationObject;
    public void ApplyingData()
    {

        StartCoroutine(IncreaseCount());
    }
    IEnumerator IncreaseCount()
    {
        dayValue.text = PlayerPrefs.GetInt("DayStreak", 0).ToString();
        DailyChallange.Instance.ChangesDayText(dayText);
        yield return new WaitForSeconds(0.5f);
        dayValue.text = (PlayerPrefs.GetInt("DayStreak", 0) + 1).ToString();
        if((PlayerPrefs.GetInt("DayStreak", 0) + 1) > 1)
        {
            dayText.text = "Days";
        }
        else
        {
            dayText.text = "Day";
        }
        DailyChallange.Instance.IncrementStreakOnWin();
    }
   

}

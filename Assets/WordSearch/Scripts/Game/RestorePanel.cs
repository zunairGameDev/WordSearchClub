using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestorePanel : MonoBehaviour
{
    public TextMeshProUGUI dayValue;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI cautionText;

    public void ApplyData()
    {
        dayValue.text = PlayerPrefs.GetInt("DayStreak", 0).ToString();
        DailyChallange.Instance.ChangesDayText(dayText);
        cautionText.text = "Your " + PlayerPrefs.GetInt("", 0).ToString() + " days streak was broken! You can restore it or start over from 0";
    }
}

using BBG.WordSearch;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DailyChallange : MonoBehaviour
{
    public List<string> missingWordsQoutes;
    public TextMeshProUGUI calaenderText;
    public TextMeshProUGUI QouteText;
    public TextMeshProUGUI AuthorName;
    public Image leafImage;
    public TextMeshProUGUI timer;
    public GameObject timerObject;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI bestStreakText;
    public TextMeshProUGUI motivationalText;
    public GameObject secondTimerWithText;
    public GameObject secondTimerReplacerText;
    public TextMeshProUGUI timer1;
    private DateTime endTime;
    public bool isPlayedToday;

    void Start()
    {
        LoadEndTime();
    }

    void Update()
    {
        // Calculate remaining time by subtracting current time from the end time
        TimeSpan remainingTime = endTime - DateTime.Now;
        calaenderText.text = DateTime.Now.ToString("ddd dd MMM");
        if (remainingTime.TotalSeconds > 0)
        {
            // Update the UI Text if it exists
            if (timer != null)
            {
                timer.text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                timer1.text = $" {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            }
        }
        else
        {
            PlayerPrefs.DeleteKey("QouteUpdatedText");
            if (PlayerPrefs.GetInt("DailyChallange") < GameManager.Instance.dailyLevelfiles.Count - 1)
            {
                PlayerPrefs.SetInt("DailyChallange", PlayerPrefs.GetInt("DailyChallange") + 1);

            }
            else
            {
                PlayerPrefs.SetInt("DailyChallange", 0);
            }
            if (PlayerPrefs.GetInt("IsTodayPlay") == 0)
            {
                PlayerPrefs.SetInt("DayStreak", 0);
            }
            // Timer has expired, reset the countdown
            timer.text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            timer1.text = $" {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            ApplyingData();
            ResetTimer();
        }
    }
    public void ResetDayStreak()
    {
        PlayerPrefs.SetInt("DayStreak", 0);
    }
    public void ApplyingData()
    {
        QouteText.text = PlayerPrefs.GetString("QouteUpdatedText", missingWordsQoutes[PlayerPrefs.GetInt("DailyChallange")]);
        AuthorName.text = GameManager.Instance.dailyLevelfiles[PlayerPrefs.GetInt("DailyChallange")].name;
        streakText.text = PlayerPrefs.GetInt("DayStreak", 0).ToString();
        bestStreakText.text = "Best: " + PlayerPrefs.GetInt("BestStreak", 0).ToString() + " Days";
        if (PlayerPrefs.GetInt("DayStreak", 0) == 0)
        {
            motivationalText.text = "Start your streak by solving today's puzzle!";
        }
        else
        {
            motivationalText.text = "Wonderful! Play everyday and grow your streak";
        }
        if (PlayerPrefs.GetInt("IsTodayPlay") == 0)
        {
            secondTimerReplacerText.SetActive(false);
            secondTimerWithText.SetActive(true);
        }
        else
        {
            secondTimerWithText.SetActive(false);
            secondTimerReplacerText.SetActive(true);
        }
    }

    private void ResetTimer()
    {
        DateTime now = DateTime.Now;
        DateTime midnight = now.Date.AddDays(1); // Sets midnight to the start of the next day
        endTime = midnight;
        SaveEndTime();
    }

    private void LoadEndTime()
    {
        if (PlayerPrefs.HasKey("EndTime"))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString("EndTime"));
            endTime = DateTime.FromBinary(temp);
        }
        else
        {
            ResetTimer(); // Initialize with a new timer if there's no saved end time
        }
    }

    private void SaveEndTime()
    {
        PlayerPrefs.SetString("EndTime", endTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        SaveEndTime(); // Save the end time when the application quits
    }
}

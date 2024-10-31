using BBG;
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
    public static DailyChallange Instance;
    public List<string> missingWordsQoutes;
    public TextMeshProUGUI calaenderText;
    public TextMeshProUGUI QouteText;
    public TextMeshProUGUI AuthorName;
    public Image leafImage;
    public TextMeshProUGUI timer;
    public GameObject timerObject;
    public TextMeshProUGUI streakText;
    public TextMeshProUGUI streakDay;
    public TextMeshProUGUI bestStreakText;
    public TextMeshProUGUI bestStreakDay;
    public TextMeshProUGUI motivationalText;
    public GameObject secondTimerWithText;
    public GameObject secondTimerReplacerText;
    public TextMeshProUGUI timer1;
    private DateTime endTime;
    public bool isPlayedToday;

    public DailyRetsorePopup streakBreakPanel; // Panel to show when streak is broken
    public TextMeshProUGUI brokenStreakText; // Text to show broken streak info
    public Button restoreButton;
    public Button startOverButton;

    private const string DayStreakKey = "DayStreak";
    private const string BestStreakKey = "BestStreak";
    private const string LastLoginDateKey = "LastLoginDate";
    private const string EndTimeKey = "EndTime";
    private const string DailyChallengeKey = "DailyChallange";
    private const string QuoteUpdatedTextKey = "QouteUpdatedText";
    private const string IsTodayPlayKey = "IsTodayPlay";
    private int restoreCost = 200; // Cost to restore streak

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {

        LoadEndTime();
        CheckStreak();
        ApplyingData();
        // Hide the streak break panel by default
        streakBreakPanel.Hide(false);

        // Add button listeners for restore and start over
        // restoreButton.onClick.AddListener(RestoreStreak);
        // startOverButton.onClick.AddListener(StartOverStreak);
    }

    public void OnclickChallengePanel()
    {
        LoadEndTime();
        CheckStreak();
        ApplyingData();
    }


    void Update()
    {
        TimeSpan remainingTime = endTime - DateTime.Now;
        calaenderText.text = DateTime.Now.ToString("ddd dd MMM");

        if (remainingTime.TotalSeconds > 0)
        {
            if (timer != null)
            {
                timer.text = $"{remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
                timer1.text = $" {remainingTime.Hours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
            }
        }
        else
        {
            Debug.Log("Rest");
            // Reset daily challenge and check streak only at the end of the day
            ResetDailyChallenge();
            CheckStreak();
            ResetTimer();
            ApplyingData();
        }
    }
    public void IncrementStreakOnWin()
    {
        int currentStreak = PlayerPrefs.GetInt(DayStreakKey, 0);
        int bestStreak = PlayerPrefs.GetInt(BestStreakKey, 0);

        currentStreak++;
        PlayerPrefs.SetInt(DayStreakKey, currentStreak);

        // Update best streak if current streak exceeds it
        if (currentStreak > bestStreak)
        {
            bestStreak = currentStreak;
            PlayerPrefs.SetInt(BestStreakKey, bestStreak);
        }

        SaveLastLoginDate(DateTime.Now.Date); // Save today's date as the last played date
        Debug.Log("SavingDatathere");
        ApplyingData(); // Update UI
    }

    private void CheckStreak()
    {
        DateTime currentDate = DateTime.Now.Date;
        DateTime lastLoginDate = LoadLastLoginDate();
        Debug.Log(currentDate + "CurrentDate");
        Debug.Log(lastLoginDate + "lastLoginDate");
        int currentStreak = PlayerPrefs.GetInt(DayStreakKey, 0);
        int bestStreak = PlayerPrefs.GetInt(BestStreakKey, 0);

        
        if (lastLoginDate != currentDate && lastLoginDate != currentDate.AddDays(-1)&& lastLoginDate!= DateTime.MinValue)
        {
            // If the last login is not yesterday, the streak is broken
            ShowStreakBreakPanel(currentStreak);
            currentStreak = 0; // Reset streak to 0 if broken
            PlayerPrefs.SetInt(DayStreakKey, currentStreak);
        }

        // Update UI text
        streakText.text = $"{currentStreak}";
        ChangesDayText(streakDay);
        bestStreakText.text = "Best: " + bestStreak;
        ChangesBestDayText(bestStreakDay);
        // Set motivational message
        motivationalText.text = currentStreak == 0
            ? "Start your streak by solving today's puzzle!"
            : "Wonderful! Play every day and grow your streak";
    }


    private void ResetTimer()
    {
        DateTime now = DateTime.Now;
        DateTime midnight = now.Date.AddDays(1);
        endTime = midnight;
        SaveEndTime();
    }

    private void LoadEndTime()
    {
        if (PlayerPrefs.HasKey(EndTimeKey))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(EndTimeKey));
            endTime = DateTime.FromBinary(temp);
        }
        else
        {
            ResetTimer();
        }
    }

    private void SaveEndTime()
    {
        PlayerPrefs.SetString(EndTimeKey, endTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }
    void ResetDailyChallenge()
    {
        PlayerPrefs.DeleteKey(QuoteUpdatedTextKey);

        int dailyChallengeIndex = PlayerPrefs.GetInt(DailyChallengeKey, 0);
        if (dailyChallengeIndex < GameManager.Instance.dailyLevelfiles.Count - 1)
        {
            PlayerPrefs.SetInt(DailyChallengeKey, dailyChallengeIndex + 1);
        }
        else
        {
            PlayerPrefs.SetInt(DailyChallengeKey, 0);
        }

        PlayerPrefs.SetInt(IsTodayPlayKey, 0);
    }
    private DateTime LoadLastLoginDate()
    {
        if (PlayerPrefs.HasKey(LastLoginDateKey))
        {
            long temp = Convert.ToInt64(PlayerPrefs.GetString(LastLoginDateKey));
            return DateTime.FromBinary(temp);
        }
        return DateTime.MinValue;
    }

    private void SaveLastLoginDate(DateTime date)
    {
        PlayerPrefs.SetString(LastLoginDateKey, date.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void ShowStreakBreakPanel(int brokenStreak)
    {

        PopupManager.Instance.Show("RestorePanel");
        //brokenStreakText.text = $"Your {brokenStreak} day streak was broken! You can restore it or start over from 0.";
    }

    public void RestoreStreak()
    {
        int playerCoins = PlayerPrefs.GetInt("Coins", 0); // Assuming coins are saved in PlayerPrefs
        if (playerCoins >= restoreCost)
        {
            PlayerPrefs.SetInt("Coins", playerCoins - restoreCost); // Deduct coins
            int currentStreak = PlayerPrefs.GetInt(DayStreakKey, 0);
            currentStreak = Math.Max(currentStreak, 1); // Restore to at least 1 day
            PlayerPrefs.SetInt(DayStreakKey, currentStreak);

            streakBreakPanel.Hide(false); // Hide panel after restoring
            ApplyingData();
        }
        else
        {
            Debug.Log("Not enough coins to restore streak.");
        }
    }
    public void ChangesDayText(TextMeshProUGUI text)
    {
        if (PlayerPrefs.GetInt("DayStreak", 0) > 1)
        {
            text.text = "Days";
        }
        else
        {
            text.text = "Day";
        }
    }
    public void ChangesBestDayText(TextMeshProUGUI text)
    {
        if (PlayerPrefs.GetInt(BestStreakKey, 0) > 1)
        {
            text.text = "Days";
        }
        else
        {
            text.text = "Day";
        }
    }

    public void StartOverStreak()
    {
        PlayerPrefs.SetInt(DayStreakKey, 0); // Reset streak to 0
        streakBreakPanel.Hide(false); // Hide panel after starting over
        ApplyingData();
    }


    public void ApplyingData()
    {
        int dailyChallengeIndex = PlayerPrefs.GetInt(DailyChallengeKey);
        QouteText.text = PlayerPrefs.GetString(QuoteUpdatedTextKey, missingWordsQoutes[dailyChallengeIndex]);
        AuthorName.text = GameManager.Instance.dailyLevelfiles[dailyChallengeIndex].name;
        streakText.text = PlayerPrefs.GetInt(DayStreakKey, 0).ToString();
        ChangesDayText(streakDay);
        bestStreakText.text = "Best: " + PlayerPrefs.GetInt(BestStreakKey, 0).ToString();
        ChangesBestDayText(bestStreakDay);
        motivationalText.text = PlayerPrefs.GetInt(DayStreakKey, 0) == 0
            ? "Start your streak by solving today's puzzle!"
            : "Wonderful! Play every day and grow your streak";

        if (PlayerPrefs.GetInt(IsTodayPlayKey, 0) == 0)
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


    void OnApplicationQuit()
    {
        SaveEndTime();
    }
}

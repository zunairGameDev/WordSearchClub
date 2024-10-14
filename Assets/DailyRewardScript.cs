using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DailyRewardSystemWithSlider : MonoBehaviour
{
    public GameObject pingPongGift;
    public GameObject rewardBoxParent;
    public GameObject[] rewardBoxes;
    public Text[] rewardText;


    public Button[] boxes;         // 3 Boxes for selection
    public Text cartText;          // To show the total coins in the cart
    public Text timerText;         // To show the time left for the next reward
    public Slider dayProgressSlider; // Slider to track 7-day progress
    public int[] coinRewards = { 10, 25, 50 };  // Possible coin rewards
    public GameObject gifts_panel;
    public Image coinImg;
    public GameObject rewardPanel;  // Panel to show after 1 minute

    private DateTime nextRewardTime;
    private TimeSpan remainingTime;
    private int totalCoins;
    private int dayProgress;       // Current day in 7-day streak

    private PanelController panelController;

    public int current_Index;






    void Start()
    {
        //panelController.ActivatePanel();
        // Load total coins and next reward time
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        dayProgress = PlayerPrefs.GetInt("DayProgress", 0);
        UpdateCartText(current_Index);

        // Set slider value to the current day progress
        dayProgressSlider.maxValue = 7;
        dayProgressSlider.value = dayProgress;

        if (PlayerPrefs.HasKey("NextRewardTime"))
        {
            nextRewardTime = DateTime.Parse(PlayerPrefs.GetString("NextRewardTime"));
        }
        else
        {
            nextRewardTime = DateTime.Now;
        }

        CheckRewardEligibility();
    }

    void Update()
    {
        UpdateTimerUI();
    }

    void CheckRewardEligibility()
    {
        // Check if 24 hours have passed since the last reward
        if (DateTime.Now >= nextRewardTime)
        {
            EnableBoxes();
        }
        else
        {
            DisableBoxes();
        }
    }

    void EnableBoxes()
    {
        pingPongGift.SetActive(true);
    }

    void DisableBoxes()
    {
        pingPongGift.SetActive(false);

    }

    public void SelectBox(int val)
    {
        // Generate a random reward
        int randomIndex = UnityEngine.Random.Range(0, coinRewards.Length);
        int reward = coinRewards[randomIndex];
        current_Index = val;
        // Show reward on the selected box button
        //selectedBox.transform.GetChild(0).GetComponent<Text>().text = "You got " + reward + " coins!";

        // Add the reward to total coins and update the cart text
        totalCoins += reward;
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        UpdateCartText(val);

        // Update the slider progress for 7-day rewards
        dayProgress++;
        if (dayProgress > 7)
        {
            dayProgress = 1;  // Reset to 1 after 7 days
        }
        dayProgressSlider.value = dayProgress;
        PlayerPrefs.SetInt("DayProgress", dayProgress);

        // Set the next reward time to 1 minute later for testing (change to 24 hours later in actual use)
        nextRewardTime = DateTime.Now.AddMinutes(1);
        PlayerPrefs.SetString("NextRewardTime", nextRewardTime.ToString());

        // Start coroutine to hide text after 3 seconds
        StartCoroutine(HideTextAfterDelay(val));

        // Start coroutine to show reward panel after 1 minute
        StartCoroutine(ShowRewardAfterOneMinute());
    }

    IEnumerator HideTextAfterDelay(int val)
    {

        // Wait for 3 seconds
        yield return new WaitForSeconds(3f);
        pingPongGift.SetActive(false);
        for (int i = 0; i < rewardBoxes.Length; i++)
        {
            rewardBoxes[i].SetActive(false);

        }
        rewardBoxes[val].SetActive(true);
        rewardBoxParent.SetActive(true);

    }

    IEnumerator ShowRewardAfterOneMinute()
    {
        // Wait for 1 minute
        yield return new WaitForSeconds(60f);

        // Show the reward panel
        rewardPanel.SetActive(true);
    }

    void UpdateCartText(int val)
    {
        rewardText[val].text = "+" + totalCoins;
        // Update the total coins in the cart
        cartText.text = "+" + totalCoins;
        coinImg.gameObject.SetActive(true);
    }

    void UpdateTimerUI()
    {
        // Calculate the remaining time for the next reward
        remainingTime = nextRewardTime - DateTime.Now;

        if (remainingTime.TotalSeconds > 0)
        {
            timerText.text = string.Format("until next reward {0:D2}:{1:D2}:{2:D2}",
               remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
        }
        else
        {
            timerText.text = "You can collect your reward!";
            // Disable automatic box enabling
            // EnableBoxes();  // Commented out to prevent auto-showing boxes after 1 minute
        }
    }

    IEnumerator GiftPanel()
    {
        yield return new WaitForSeconds(3f);
        gifts_panel.SetActive(false);
        yield return new WaitForSeconds(1f);
        //PanelController.Panelinstance.gameObject.SetActive(true);
        PanelController.Panelinstance.ActivatePanel();
        yield return new WaitForSeconds(3f);
        PanelController.Panelinstance.DeactivatePanel();
    }
}

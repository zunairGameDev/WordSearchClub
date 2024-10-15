using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
public class DailyRewardSystemWithSlider : MonoBehaviour
{
    public GameObject pingPongGift;
    public GameObject[] boxesToMove;
    public GameObject rewardBoxParent;
    public GameObject[] rewardBoxes;
    public RectTransform boxCenterPosition;
    private Vector2[] originalPositions;
    private Vector2[] originalScales;
    public Text[] rewardText;
    public Text cartText;          // To show the total coins in the cart
    public Text timerText;         // To show the time left for the next reward
    public Slider dayProgressSlider; // Slider to track 7-day progress
    public int[] coinRewards = { 10, 25, 50 };  // Possible coin rewards
    public Image coinImg;
    public GameObject rewardPanel;  // Panel to show after 1 minute
    private DateTime nextRewardTime;
    private TimeSpan remainingTime;
    private int totalCoins;
    private int dayProgress;       // Current day in 7-day streak
    public int current_Index;
    private float timerDuration = 3f;
    public List<Sprite> box_Top;
    public List<Sprite> frontSprit;
    public List<Sprite> boxSprit;
    public Image boxTop;
    public Image front;
    public Image box;
    public GameObject waitForNextReward;
    private void OnEnable()
    {
        //panelController.ActivatePanel();
        // Load total coins and next reward time
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        dayProgress = PlayerPrefs.GetInt("DayProgress", 0);
        UpdateCartText(current_Index);
        // Set slider value to the current day progress
        dayProgressSlider.maxValue = 7;
        dayProgressSlider.value = dayProgress;
        // Initialize the originalPositions array with the same length as boxesToMove
        originalPositions = new Vector2[boxesToMove.Length];
        originalScales = new Vector2[boxesToMove.Length];
        // Store the original positions
        for (int i = 0; i < boxesToMove.Length; i++)
        {
            // Get the RectTransform of the GameObject and store its anchored position
            RectTransform rect = boxesToMove[i].GetComponent<RectTransform>();
            if (rect != null)
            {
                originalPositions[i] = rect.anchoredPosition;
                originalScales[i] = rect.anchoredPosition;
            }
        }
        if (PlayerPrefs.HasKey("NextRewardTime"))
        {
            nextRewardTime = DateTime.Parse(PlayerPrefs.GetString("NextRewardTime"));
            rewardBoxParent.SetActive(false);
            WaitForNewRewardBox();
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
    public void WaitForNewRewardBox()
    {
        boxTop.sprite = box_Top[PlayerPrefs.GetInt("LastRewardValue")];
        front.sprite = frontSprit[PlayerPrefs.GetInt("LastRewardValue")];
        box.sprite = boxSprit[PlayerPrefs.GetInt("LastRewardValue")];
        waitForNextReward.SetActive(true);
    }
    void CheckRewardEligibility()
    {
        // Check if 24 hours have passed since the last reward
        if (DateTime.Now >= nextRewardTime)
        {
            EnableBoxes();
            waitForNextReward.SetActive(false);
        }
        else
        {
            DisableBoxes();
            WaitForNewRewardBox();
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
        PlayerPrefs.SetInt("LastRewardValue", val);
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
        nextRewardTime = DateTime.Now.AddMinutes(24);
        PlayerPrefs.SetString("NextRewardTime", nextRewardTime.ToString());
        pingPongGift.SetActive(true);
        // Start coroutine to hide text after 3 seconds
        StartCoroutine(HideTextAfterDelay(val));
        // Start coroutine to show reward panel after 1 minute
        StartCoroutine(ShowRewardAfterOneMinute());
    }
    IEnumerator HideTextAfterDelay(int val)
    {
        // Wait for 3 seconds
        for (int i = 0; i < boxesToMove.Length; i++)
        {
            boxesToMove[i].SetActive(false);
        }
        boxesToMove[val].SetActive(true);
        pingPongGift.SetActive(false);
        MoveBox(boxesToMove[val].transform);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < rewardBoxes.Length; i++)
        {
            rewardBoxes[i].SetActive(false);
        }
        rewardBoxes[val].SetActive(true);
        rewardBoxParent.SetActive(true);
    }
    public void MoveBox(Transform box)
    {
        // Move the selected button to the panel's center
        box.DOScale(boxCenterPosition.GetChild(0).localScale, 0.5f).SetEase(Ease.InOutSine);
        box.DOMove(boxCenterPosition.position, 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            box.gameObject.SetActive(false);
            // Wait for the timer and move the button back after the timer finishes
            Invoke(nameof(ResetButtons), timerDuration);
        });
    }
    private void ResetButtons()
    {
        // Move the selected button back to its original position
        for (int i = 0; i < boxesToMove.Length; i++)
        {
            RectTransform rectPosition = boxesToMove[i].GetComponent<RectTransform>();
            rectPosition.DOAnchorPos(originalPositions[i], 0.5f);
            rectPosition.transform.localScale = Vector3.one;
            boxesToMove[i].SetActive(false); // Activate all buttons again
        }
    }
    IEnumerator ShowRewardAfterOneMinute()
    {
        yield return new WaitForSeconds(3.10f);
        GlobalData.CoinCount = GlobalData.CoinCount + totalCoins;
        MainMenuText.Instance.coinsText.text = GlobalData.CoinCount.ToString();
        // Wait for 1 minute
        //yield return new WaitForSeconds(86400f);
        yield return new WaitForSeconds(60f);
        // Show the reward panel
        pingPongGift.SetActive(true);
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
            EnableBoxes();  // Commented out to prevent auto-showing boxes after 1 minute
            rewardBoxParent.SetActive(false);
            waitForNextReward.SetActive(false);
        }
    }
}
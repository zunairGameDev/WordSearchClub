using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuText : MonoBehaviour
{
    public static MainMenuText Instance;
    public GameObject gameManager;
    public CountryInfo countryInfo;
    public TextMeshProUGUI mainMenuPlayButton;
    public TextMeshProUGUI dailyPlayButton;
    public TextMeshProUGUI collectionPlayButton;
    public TextMeshProUGUI countryName;
    public Slider sliderProgess;
    public int maxValue;
    public int currentValue;
    public TextMeshProUGUI sliderText;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        TextUpdating();
        countryInfo = gameManager.GetComponent<GameManager>().countryInfo[PlayerPrefs.GetInt("CountryInfoValue")];
        FillAmount();

    }
    public void UpdateLeveInfo() 
    {
        
    }
    private void TextUpdating()
    {
        mainMenuPlayButton.text = "Play Level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        dailyPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        collectionPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
    }

    public void FillAmount()
    {
        if (currentValue >= countryInfo.maxValue)
        {
            PlayerPrefs.SetInt("CountryInfoValue", PlayerPrefs.GetInt("CountryInfoValue") + 1);
            TextUpdating();
            countryInfo = gameManager.GetComponent<GameManager>().countryInfo[PlayerPrefs.GetInt("CountryInfoValue")];
            PlayerPrefs.SetInt("CurrentValue", 0);
        }
        currentValue = PlayerPrefs.GetInt("CurrentValue");
        countryName.text = countryInfo.CountryName;
        float fillAmount = (float)currentValue / countryInfo.maxValue;
        sliderProgess.value = fillAmount;
        sliderText.text = currentValue.ToString() + " / " + countryInfo.maxValue.ToString();

    }
}

using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuText : MonoBehaviour
{
    public static MainMenuText Instance;
    //public GameObject gameManager;
    public CountryInfo countryInfo;
    public TextMeshProUGUI mainMenuPlayButton;
    public TextMeshProUGUI mainMenuShadowPlayButton;
    public TextMeshProUGUI dailyPlayButton;
    public TextMeshProUGUI dailyShadowPlayButton;
    public TextMeshProUGUI collectionPlayButton;
    public TextMeshProUGUI collectionShadowPlayButton;
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
        UpdateLeveInfo();

    }

    public void UpdateLeveInfo()
    {
        TextUpdating();
        countryInfo = /*gameManager.GetComponent<GameManager>()*/GameManager.Instance.countryInfo[PlayerPrefs.GetInt("CountryInfoValue")];
        FillAmount();

    }

    private void TextUpdating()
    {
        mainMenuPlayButton.text = "Play Level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        mainMenuShadowPlayButton.text = mainMenuPlayButton.text;
        //dailyPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        collectionPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        collectionShadowPlayButton.text = collectionPlayButton.text;
    }

    public void FillAmount()
    {
        if (currentValue >= countryInfo.maxValue)
        {
            PlayerPrefs.SetInt(countryInfo.countryName, currentValue);
            PlayerPrefs.SetInt("CountryInfoValue", PlayerPrefs.GetInt("CountryInfoValue") + 1);
            TextUpdating();
            countryInfo = GameManager.Instance.countryInfo[PlayerPrefs.GetInt("CountryInfoValue")];
            PlayerPrefs.SetInt("CurrentValue", 0);
        }
        currentValue = PlayerPrefs.GetInt("CurrentValue");
        PlayerPrefs.SetInt(countryInfo.countryName, currentValue);
        countryInfo.currentValue = currentValue;
        countryName.text = countryInfo.countryName;
        GameManager.Instance.BackGroundImage.sprite = countryInfo.BackGroundImage;
        float fillAmount = (float)currentValue / countryInfo.maxValue;
        sliderProgess.value = fillAmount;
        sliderText.text = currentValue.ToString() + " / " + countryInfo.maxValue.ToString();

    }
}

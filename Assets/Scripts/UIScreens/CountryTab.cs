using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountryTab : MonoBehaviour
{
    public TextMeshProUGUI countryName;
    public Image lockImage;
    public Image tickImage;
    public TextMeshProUGUI levelInfo;
    public int currentIndex;
    public int countryLevelIndex;

    public void ApplyingData(CountryInfo countryInfo)
    {
        countryLevelIndex = PlayerPrefs.GetInt(countryInfo.countryName);
        countryLevelIndex = MainMenuText.Instance.currentValue;
        countryName.text = countryInfo.countryName;
        currentIndex = PlayerPrefs.GetInt("SelectJasonLevel");
        if (currentIndex + 1 >= countryInfo.unlockAt)
        {
            if (PlayerPrefs.GetInt(countryInfo.countryName) >= countryInfo.maxValue)
            {
                lockImage.gameObject.SetActive(false);
                tickImage.gameObject.SetActive(true);
                levelInfo.gameObject.SetActive(false);

            }
            else
            {
                lockImage.gameObject.SetActive(false);
                tickImage.gameObject.SetActive(false);
                levelInfo.text = MainMenuText.Instance.currentValue.ToString() + "/" + countryInfo.maxValue.ToString();
                
            }
        }
        else
        {
            lockImage.gameObject.SetActive(true); tickImage.gameObject.SetActive(false);
            levelInfo.text = "Level " + countryInfo.unlockAt.ToString();
        }

    }
}

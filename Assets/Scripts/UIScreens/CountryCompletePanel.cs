using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountryCompletePanel : MonoBehaviour
{
    public CountryInfo countryInfo;
    public Image countryFlagImage;
    public TextMeshProUGUI countryName;
    public Image countryStamp;
    public TextMeshProUGUI coinsText;
    

    public void ApplyingData()
    {
        countryInfo = GameManager.Instance.countryInfo[PlayerPrefs.GetInt("CountryStamp", 0)];
        //countryName.text = countryInfo.countryName;
        //countryFlagImage.sprite = countryInfo.countryFlag;
        countryStamp.sprite = countryInfo.countryStamp;
        // Coins Update
        coinsText.text = "+25";
        this.gameObject.SetActive(true);
    }
    public void OnClickClosePane()
    {
        GlobalData.CoinCount = GlobalData.CoinCount + 25;
        MainMenuText.Instance.coinsText.text = GlobalData.CoinCount.ToString();
        WinPanelController.Instance.NewDestinationPanel.GetComponent<NewDestinationPanel>().ApplyNextCountryData();
        WinPanelController.Instance.NewDestinationPanel.SetActive(true);
        PlayerPrefs.SetInt("CountryStamp", PlayerPrefs.GetInt("CountryStamp") + 1);        
        this.gameObject.SetActive(false);
    }

}

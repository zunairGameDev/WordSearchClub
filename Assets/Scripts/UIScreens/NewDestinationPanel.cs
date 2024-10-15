using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewDestinationPanel : MonoBehaviour
{
    public Button nextButton;
    public Image newDestination;
    public Image countryFlag;
    public Text countryName;

    private void OnDisable()
    {

    }
    private void OnEnable()
    {
        Debug.Log("C");
        StartCoroutine(PlayNextLevel());
        StartCoroutine(WaitToChangeBG());
    }
    public void ApplyNextCountryData()
    {
        nextButton = WinPanelController.Instance.nextLevelButton;
        countryFlag.sprite = MainMenuText.Instance.countryInfo.countryFlag;
        newDestination.sprite = MainMenuText.Instance.countryInfo.BackGroundImage;
        countryName.text = MainMenuText.Instance.countryInfo.countryName;
        
    }

    IEnumerator PlayNextLevel()
    {
        Debug.Log("A");
        yield return new WaitForSeconds(4f);
        nextButton.onClick.Invoke();
        Debug.Log("B");
        //this.gameObject.SetActive(false);
    }
    IEnumerator WaitToChangeBG()
    {
        yield return new WaitForSeconds(3.05f);
        GameManager.Instance.BackGroundImage.sprite = MainMenuText.Instance.countryInfo.BackGroundImage;
    }
}

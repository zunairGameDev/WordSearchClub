using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomDestinationPanel : MonoBehaviour
{
    public Button nextButton;
    public Image newDestination;
    public Image countryFlag;
    public TextMeshProUGUI countryName;

    private void OnDisable()
    {

    }
    private void OnEnable()
    {
        Debug.Log("C");
        StartCoroutine(PlayNextLevel());
    }
    public void ApplyNextCountryData()
    {
        nextButton = WinPanelController.Instance.nextLevelButton;
        countryFlag.sprite = MainMenuText.Instance.countryInfo.countryFlag;
        newDestination.sprite = MainMenuText.Instance.countryInfo.BackGroundImage;

    }

    IEnumerator PlayNextLevel()
    {

        yield return new WaitForSeconds(3f);
        nextButton.onClick.Invoke();
        this.gameObject.SetActive(false);
    }
}

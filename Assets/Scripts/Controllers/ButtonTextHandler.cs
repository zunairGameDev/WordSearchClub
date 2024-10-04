using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextHandler : MonoBehaviour
{
    private bool isSwitchingPanel = false;
    public TextMeshProUGUI homeText;
    public TextMeshProUGUI collectionText;
    public TextMeshProUGUI challengeText;
    public List<CanvasGroup> mainPanelCanvas;
    public List<Animator> mainMenuButton;

    // Start mein ensure karen ke text hidden ho
    void Start()
    {
        homeText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(false);
        ShowPanel(0);
        PanelAnimationShow(0);
    }
    public void ShowPanel(int value)
    {
        if (isSwitchingPanel)
            return;  // Prevent switching if already switching

        StartCoroutine(SwitchPanel(value));
        //for (int i = 0; i < mainPanelCanvas.Count; i++)
        //{
        //    mainPanelCanvas[i].alpha = 0;
        //    mainPanelCanvas[i].interactable = false;
        //    mainPanelCanvas[i].blocksRaycasts = false;
        //}
        //mainPanelCanvas[value].alpha = 1;
        //mainPanelCanvas[value].interactable = true;
        //mainPanelCanvas[value].blocksRaycasts = true;
    }
    private IEnumerator SwitchPanel(int value)
    {
        isSwitchingPanel = true;

        // First, hide all panels
        for (int i = 0; i < mainPanelCanvas.Count; i++)
        {
            mainPanelCanvas[i].alpha = 0;
            mainPanelCanvas[i].interactable = false;
            mainPanelCanvas[i].blocksRaycasts = false;
        }

        // Wait a small amount of time if necessary to ensure there's no overlapping issues
        

        // Now, show the selected panel
        mainPanelCanvas[value].alpha = 1;
        mainPanelCanvas[value].interactable = true;
        mainPanelCanvas[value].blocksRaycasts = true;
        yield return new WaitForEndOfFrame();
        isSwitchingPanel = false; // Done switching
    }

    public void PanelAnimationShow(int value)
    {
        for (int i = 0; i < mainMenuButton.Count; i++)
        {
            mainMenuButton[i].SetTrigger("Normal");
        }
        mainMenuButton[value].SetTrigger("Selected");
    }
    // Home button ka function
    public void OnHomeButtonClick()
    {
        homeText.gameObject.SetActive(true);
        collectionText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(false);
    }

    // Collection button ka function
    public void OnCollectionButtonClick()
    {
        homeText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(true);
        challengeText.gameObject.SetActive(false);
    }

    // Challenge button ka function
    public void OnChallengeButtonClick()
    {
        homeText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(true);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextHandler : MonoBehaviour
{
    public Text homeText;
    public Text collectionText;
    public Text challengeText;
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
        for (int i = 0; i < mainPanelCanvas.Count; i++)
        {
            mainPanelCanvas[i].alpha = 0;
            mainPanelCanvas[i].interactable = false;
            mainPanelCanvas[i].blocksRaycasts = false;
        }
        mainPanelCanvas[value].alpha = 1;
        mainPanelCanvas[value].interactable = true;
        mainPanelCanvas[value].blocksRaycasts = true;
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

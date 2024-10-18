using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextHandler : MonoBehaviour
{
    private bool isSwitchingPanel = false;
    public TextMeshProUGUI homeText;
    public TextMeshProUGUI homeShadowText;
    public TextMeshProUGUI collectionText;
    public TextMeshProUGUI collectionShadowText;
    public TextMeshProUGUI challengeText;
    public TextMeshProUGUI challengeShadowText;
    public List<CanvasGroup> mainPanelCanvas;
    public List<Animator> mainMenuButton;
    public float cooldownTime = 0.5f; // Cooldown duration in seconds
    private float lastClickTime;

    // Start mein ensure karen ke text hidden ho
    void Start()
    {
        homeText.gameObject.SetActive(true);
        homeShadowText.gameObject.SetActive(true);
        collectionText.gameObject.SetActive(false);
        collectionShadowText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(false);
        challengeShadowText.gameObject.SetActive(false);
        ShowPanel(0);
        PanelAnimationShow(0);
    }
    public void ShowPanel(int value)
    {
        if (Time.time >= lastClickTime + cooldownTime)
        {
            // Perform your action here
            Debug.Log("Action executed!");

            // Update the last click time
            lastClickTime = Time.time;
            SwitchPanel(value);
            PanelAnimationShow(value);
        }
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
    private void SwitchPanel(int value)
    {
        

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
        
    }

    public void PanelAnimationShow(int value)
    {
        if (value >= mainMenuButton.Count)
            return;
        for (int i = 0; i < mainMenuButton.Count; i++)
        {
            mainMenuButton[i].SetTrigger("Normal");
        }
        mainMenuButton[value].SetTrigger("Selected");
        SwitchPanelText(value);
    }
    public void SwitchPanelText(int value)
    {
        switch (value)
        {
            case 0:
                OnHomeButtonClick();
                break;
            case 1:
                OnChallengeButtonClick();
                break;
            case 2:
                OnCollectionButtonClick();
                break;
            default:
                break;
        }
    }
    // Home button ka function
    public void OnHomeButtonClick()
    {
        homeText.gameObject.SetActive(true);
        homeShadowText.gameObject.SetActive(true);
        collectionText.gameObject.SetActive(false);
        collectionShadowText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(false);
        challengeShadowText.gameObject.SetActive(false);
    }

    // Collection button ka function
    public void OnCollectionButtonClick()
    {
        homeText.gameObject.SetActive(false);
        homeShadowText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(true);
        collectionShadowText.gameObject.SetActive(true);
        challengeText.gameObject.SetActive(false);
        challengeShadowText.gameObject.SetActive(false);
    }

    // Challenge button ka function
    public void OnChallengeButtonClick()
    {
        homeText.gameObject.SetActive(false);
        homeShadowText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(false);
        collectionShadowText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(true);
        challengeShadowText.gameObject.SetActive(true);
    }
}

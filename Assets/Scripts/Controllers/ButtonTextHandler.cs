using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextHandler : MonoBehaviour
{
    public Text homeText;
    public Text collectionText;
    public Text challengeText;
    public List<Animator> mainMenuButton;

    // Start mein ensure karen ke text hidden ho
    void Start()
    {
        homeText.gameObject.SetActive(false);
        collectionText.gameObject.SetActive(false);
        challengeText.gameObject.SetActive(false);
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuText : MonoBehaviour
{
    public TextMeshProUGUI mainMenuPlayButton;
    public TextMeshProUGUI dailyPlayButton;
    public TextMeshProUGUI collectionPlayButton;

    private void OnEnable()
    {
        TextUpdating();
    }
    private void TextUpdating()
    {
        mainMenuPlayButton.text = "Play Level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        dailyPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
        collectionPlayButton.text = "Back to level " + (PlayerPrefs.GetInt("SelectJasonLevel") + 1).ToString();
    }
}

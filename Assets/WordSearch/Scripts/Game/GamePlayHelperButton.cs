using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GamePlayHelperButton : MonoBehaviour
{
    public GameObject starterCount;
    public TextMeshProUGUI startText;
    public GameObject hintPrice;
    public void HintButtonUpdate()
    {
        if (PlayerPrefs.GetInt("StarterCounts", 2) > 0)
        {
            startText.text = PlayerPrefs.GetInt("StarterCounts", 2).ToString();
            hintPrice.SetActive(false);
            starterCount.SetActive(true);
        }
        else
        {
            starterCount.SetActive(false);
            hintPrice.SetActive(true);
        }
    }
}

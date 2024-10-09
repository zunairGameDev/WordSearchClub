using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangingHeaderButton : MonoBehaviour
{
    public GameObject mainBackButton;
    public GameObject mainSettingButton;
    public GameObject gameplayBackButton;

    public void MainToPlay()
    {
        mainBackButton.SetActive(false);
        mainSettingButton.SetActive(false);
        gameplayBackButton.SetActive(true);
    }
    public void PlayToMain()
    {
        gameplayBackButton.SetActive(false);
        mainBackButton.SetActive(true);
        mainSettingButton.SetActive(true);
    }
}

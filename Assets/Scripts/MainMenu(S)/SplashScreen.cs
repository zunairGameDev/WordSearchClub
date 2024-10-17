using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public int toDelay;

    private void Start()
    {
       
        StartCoroutine(ToEnableLoading());
    }
    IEnumerator ToEnableLoading()
    {
        yield return new WaitForSeconds(toDelay);
        loadingPanel.SetActive(true);
        this.gameObject.SetActive(false);
    }
}

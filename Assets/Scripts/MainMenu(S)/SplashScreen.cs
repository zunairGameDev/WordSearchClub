using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public LoadingScreen loading_Panel;
    public int toDelay;

    private void Start()
    {

        StartCoroutine(ToEnableLoading());
    }
    IEnumerator ToEnableLoading()
    {
        yield return new WaitForSeconds(toDelay);
        ChaningePanel(loadingPanel.GetComponent<CanvasGroup>(), true);
        loading_Panel.GetComponent<LoadingScreen>().StartLoading();
        ChaningePanel(this.gameObject.GetComponent<CanvasGroup>(), false);

    }
    public void ChaningePanel(CanvasGroup canvas, bool toShow)
    {
        if (toShow)
        {
            canvas.alpha = 1;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;

        }
        else
        {
            canvas.alpha = 0;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;

        }
    }
}

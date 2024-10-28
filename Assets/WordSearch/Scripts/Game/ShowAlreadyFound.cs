using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowAlreadyFound : MonoBehaviour
{
    public Image alreadyFound;
    public TextMeshProUGUI text;
    public float toShowDuration;
    public float toWaitDuration;

    public void ShowPanel()
    {
        StartCoroutine(FadeInOut());
    }
    public IEnumerator FadeInOut()
    {
        yield return Fade(0f,1f, toShowDuration);
        yield return new WaitForSeconds(toWaitDuration);
        yield return Fade(1f,0f, toShowDuration);
    }
    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color startColorBG = alreadyFound.color;
        Color endColorBG = new Color(startColorBG.r, startColorBG.g, startColorBG.b, endAlpha);
        Color startColorText = text.color;
        Color endColorText = new Color(startColorText.r, startColorText.g, startColorText.b, endAlpha);

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime/duration);
            alreadyFound.color = new Color(startColorBG.r, startColorBG.g, startColorBG.b, alpha);
            text.color = new Color(startColorText.r, startColorText.g, startColorText.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        alreadyFound.color = new Color(startColorBG.r, startColorBG.g, startColorBG.b, endAlpha);
        text.color = new Color(startColorText.r, startColorText.g, startColorText.b, endAlpha);
    }
}

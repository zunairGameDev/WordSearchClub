using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextShowInCollection : MonoBehaviour
{
    public TextMeshProUGUI textdetails;
    public CanvasGroup canvasGroup;

    private void OnEnable()
    {
        canvasGroup.alpha = 0f;
        StartCoroutine(ToDisable());
    }
    IEnumerator ToDisable()
    {
        StartCoroutine(FadeInAlpha(0.3f));
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeOut(0.3f));
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }
    private IEnumerator FadeInAlpha(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / duration);
            yield return null; // Wait for the next frame
        }

        canvasGroup.alpha = 1f; // Ensure it ends at 1
    }
    private IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / duration));
            yield return null; // Wait for the next frame
        }

        canvasGroup.alpha = 0f; // Ensure it ends at 0
    }
}

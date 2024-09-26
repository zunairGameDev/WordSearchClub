using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineMove : MonoBehaviour
{
    public Transform imageScale;
    public Transform textScale;
    public Transform shineObject;
    public Transform initialPosition;
    public Transform finalPosition;

    private void OnEnable()
    {
        imageScale.GetComponent<CanvasGroup>().alpha = 1f;
        textScale.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        imageScale.localScale = new Vector3(1f, 0.5f, 1);
        shineObject.localPosition = initialPosition.localPosition;
        ShowAnimation();
    }

    public void ShowAnimation()
    {
        textScale.DOScale(new Vector3(1.11f, 1.11f, 1.11f), 0.7f).SetEase(Ease.Linear);
        imageScale.DOScale(new Vector3(1.3f, 1.44f, 1), 0.7f).SetEase(Ease.Linear).OnComplete(() =>
        {
            shineObject.DOLocalMove(finalPosition.localPosition, 1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                //imageScale.GetComponent<CanvasGroup>().alpha.Doval
                StartCoroutine(Start_Fading());
                //transform.gameObject.SetActive(false);
            });
        });
    }
    IEnumerator Start_Fading()
    {
        CanvasGroup canvasGroup = imageScale.GetComponent<CanvasGroup>();

        // Set the fade duration (for example 1 second)
        float fadeDuration = 1f;

        // While alpha is greater than 0, reduce it gradually
        while (canvasGroup.alpha > 0)
        {
            // Reduce alpha gradually (e.g., 0.05f per frame)
            canvasGroup.alpha -= Time.deltaTime / fadeDuration;

            // Wait for the next frame
            yield return null;
        }

        // Once fading is complete, set the object inactive
        transform.gameObject.SetActive(false);
    }
}


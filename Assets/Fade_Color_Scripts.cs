using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using Coffee.UIEffects; // Import DOTween

public class FadeImageWithDOTween : MonoBehaviour
{
    public Image image;  // The UI Image to tween
    public float fadeDuration = 0.1f; // Duration of the fade
    public float scaleDuration = 8f; // Duration of the fade

    void Start()
    {
        // Example: Fading out the image when the game starts
        StartCoroutine(FadeOutImage());
    }

    IEnumerator FadeOutImage()
    {
        // Tween the image's alpha value to 0 (fade out)
        
        
        //yield return new WaitForSeconds(0.5f);
        image.GetComponent<UIShiny>().Play();
        yield return new WaitForSeconds(1f);
        image.DOFade(0f, fadeDuration);
        image.transform.GetChild(0).gameObject.SetActive(false);
        //image.transform.DOScaleY(0f, scaleDuration);
        
    }

    public void FadeInImage()
    {
        // Tween the image's alpha value to 1 (fade in)
        image.DOFade(1f, fadeDuration);
        image.transform.DOScale(1f, scaleDuration);

    }
}

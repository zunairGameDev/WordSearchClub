using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;  // DOTween namespace

public class PanelImageTween : MonoBehaviour
{
    public RectTransform panelRectTransform;  // Reference to the panel's RectTransform
    public RectTransform imageRectTransform;  // Reference to the image's RectTransform
    public Text uiText;                       // Reference to the Text component

    public float imageExpandDuration = 0.5f;    // Duration for image tween
    public float textExpandDuration = 0.5f;     // Duration for text tween

    private Vector2 originalImageSize;        // Store original size of the image
    private Vector2 originalTextSize;         // Store original size of the text

    void Start()
    {
        // Store the original size of the image and text
        originalImageSize = imageRectTransform.sizeDelta;
        originalTextSize = uiText.rectTransform.sizeDelta;

        // Start the expansion tween
        ExpandImage();
    }

    public void ExpandImage()
    {
        // Get the size of the panel for tweening the image to panel size
        Vector2 targetPanelSize = panelRectTransform.sizeDelta;

        // Tween the image size to expand to the panel size
        imageRectTransform.DOSizeDelta(targetPanelSize, imageExpandDuration).OnComplete(ExpandText);
    }

    public void ExpandText()
    {
        // Calculate a slight expansion for the text, limited to certain anchors
        Vector2 targetTextSize = new(originalTextSize.x * 1.5f, originalTextSize.y * 1.5f);  // Slightly expand the text

        // Tween the text size with limited anchors
        uiText.rectTransform.DOSizeDelta(targetTextSize, textExpandDuration);
    }
}

using UnityEngine;

public class SafeAreaAdjuster : MonoBehaviour
{
    private RectTransform rectTransform;

    void Start()
    {
        // Try to get the RectTransform from the attached GameObject
        rectTransform = GetComponent<RectTransform>();

        // Check if RectTransform is assigned
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform not found! Make sure this script is attached to a UI element like a Panel or Button.");
            return;
        }

        // Apply the safe area
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        // Log some debugging info to see what's happening
        //Debug.Log("Applying safe area...");
        Rect safeArea = Screen.safeArea;

        // Log safe area details
        //Debug.Log($"Safe area rect: {safeArea}");

        // Convert safe area to normalized anchor values
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Apply these anchors to the RectTransform
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        // Log after applying anchors
        //Debug.Log("Safe area applied.");
    }
}

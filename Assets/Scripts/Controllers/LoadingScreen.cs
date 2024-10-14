using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider loadingSlider;   // Reference to the UI Slider
    public Text loadingText;       // Reference to the UI Text (optional)
    public float waitTime = 5f;    // Time in seconds to wait before loading next scene
    private float loadingProgress = 0f;
    void Start()
    {
        // Start the coroutine for the loading process
        StartCoroutine(LoadMainMenu());
    }
    IEnumerator LoadMainMenu()
    {
        // Simulate loading over time
        while (loadingProgress < 1f)
        {
            loadingProgress += Time.deltaTime / waitTime; // Adjust progress based on wait time
            loadingSlider.value = loadingProgress;        // Update slider value
            // Optional: Update loading text
            //if (loadingText != null)
            //{
            //    loadingText.text = "Loading: " + Mathf.RoundToInt(loadingProgress * 100) + "%";
            //}
            yield return null; // Wait for next frame
        }
        // After loading is complete, load the main menu scene
        loadingPanel.SetActive(false);
    }
}
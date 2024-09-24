using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuoteDisplay : MonoBehaviour
{
    public TextMeshProUGUI quoteText;
    public TextMeshProUGUI authorText; // For displaying the author's name

    public string[] quotes;
    public string[] authors; // Array to store authors corresponding to each quote

    private bool changeWithRestart;

    void Start()
    {
        // Load the boolean value from PlayerPrefs (default is false)
        changeWithRestart = PlayerPrefs.GetInt("ChangeWithRestart", 0) == 1;

        // Display a random quote if changeWithRestart is true
        if (changeWithRestart)
        {
            int randomIndex = Random.Range(0, quotes.Length);
            quoteText.text = quotes[randomIndex];
            authorText.text = " " + authors[randomIndex]; // Display the author
        }
        else
        {
            // If false, display the first quote (or a fixed one)
            quoteText.text = quotes[0];
            authorText.text = " " + authors[0]; // Display the author
        }

        // Reset the ChangeWithRestart flag for the next game start
        PlayerPrefs.SetInt("ChangeWithRestart", 0);
        PlayerPrefs.Save();

        // Start the coroutine to move to the main menu
        StartCoroutine(EnterMainMenu());
    }

    // Call this method when you want the quote to change after a restart
    public void SetChangeWithRestart(bool value)
    {
        changeWithRestart = value;
        PlayerPrefs.SetInt("ChangeWithRestart", value ? 1 : 0);
        PlayerPrefs.Save();
    }

    IEnumerator EnterMainMenu()
    {
        SetChangeWithRestart(true);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("CustomLoadingScreen");
    }
}

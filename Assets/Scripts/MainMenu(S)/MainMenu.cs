using UnityEngine;
using UnityEngine.UI;  // For handling UI elements
using UnityEngine.SceneManagement;
using TMPro;  // For scene management

public class MainMenu : MonoBehaviour
{
    #region Singleton
    public static MainMenu main_instance;
    #endregion
    [Header("Panels")]
    // Panels
    public GameObject homeScreenPanel;     
    public GameObject playerProfilePanel;  
    public GameObject sourceImagePanel;    
    public GameObject settingsPanel;       
    public GameObject dailyChallengePanel; 
    public GameObject myCollectionPanel;   
    public GameObject stampPanel;           
    public GameObject quotePanel;
    public GameObject rewardedPanel;        
    public GameObject storePanel;           
    public GameObject selectLanguagePanel;  


    [Header("UI Elements")]
    // UI Elements
    public InputField playerNameInputField;  
    public TextMeshProUGUI sourceImageInputField; 
    public Text playerNameDisplayText;      
    public Image selectedImageDisplay;       
   
    public Image backgroundImage;        
    public Sprite dailyChallengeBackground; // Sprite for the Daily Challenge background
    public Sprite defaultBackground;    
   


    // Scene management
    public void GamePlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Back button functionality
    public void OnBackButtonClick()
    {
        if (myCollectionPanel.activeSelf)
        {
            myCollectionPanel.SetActive(false);  
            homeScreenPanel.SetActive(true);     
        }
        else if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);       
            homeScreenPanel.SetActive(true);     
        }
        else if (dailyChallengePanel.activeSelf)
        {
            dailyChallengePanel.SetActive(false); 
            homeScreenPanel.SetActive(true);     
        }
        else if (rewardedPanel.activeSelf)
        {
            rewardedPanel.SetActive(false);       
            homeScreenPanel.SetActive(true);     
        }
        else if (storePanel.activeSelf)
        {
            storePanel.SetActive(false);         
            homeScreenPanel.SetActive(true);     
        }
        else
        {
            Debug.Log("No panel to close or invalid state.");
        }
    }

    // Settings panel functions
    public void OpenSettings() => settingsPanel.SetActive(true);
   

    // Daily challenge panel functions
    public void OpenDailyChallenge()
    {
        dailyChallengePanel.SetActive(true);  

        // Change the background
        if (backgroundImage != null && dailyChallengeBackground != null)
        {
            backgroundImage.sprite = dailyChallengeBackground;
        }
        else
        {
            Debug.LogWarning("Background image or Daily Challenge background sprite is not assigned.");
        }
    }

    // Open Select Language panel
    public void OpenSelectLanguagePanel()
    {
        selectLanguagePanel.SetActive(true); 
        settingsPanel.SetActive(false);       
    }

    public void OnBackFromSelectLanguage()
    {
        selectLanguagePanel.SetActive(false);  
        homeScreenPanel.SetActive(true);       
    }


    // My Collection panel functions
    public void OpenMyCollection()
    {
        myCollectionPanel.SetActive(true);
        /*sourceImagePanel.SetActive(false);*/ // Hide Source Image panel when opening My Collection
    }

    public void OpenQuotePanel()
    {
        quotePanel.SetActive(true);
        stampPanel.SetActive(false);
    }

    public void CloseStampPanel() => stampPanel.SetActive(false);
    
    public void OpenRewardedPanel() => rewardedPanel.SetActive(true);
   
    public void OpenStorePanel() => storePanel.SetActive(true);
    
    public void OnCatImageClick()
    {
        playerProfilePanel.SetActive(true);
        sourceImagePanel.SetActive(false);
    }

    // Update the Source Image panel's input field with text from Player Profile panel's input field
    public void OnNameSubmit()
    {
       
        string playerName = playerNameInputField.text;

        Debug.Log("Player Name Submitted: " + playerName);

        // Check if the name is not empty
        if (!string.IsNullOrEmpty(playerName))
        {
            // Update the player name display text
            playerNameDisplayText.text = playerName;

            // Update the source image input field with the player's name
            sourceImageInputField.text = playerName;

            // Close the player profile panel after saving the name
            playerProfilePanel.SetActive(false);

            // Optionally, you can re-enable the source image panel if needed
            sourceImagePanel.SetActive(true); // Enable if needed or keep it off
        }
        else
        {
            // Log if no name is entered
            Debug.Log("No name entered!");
        }
    }

    public void OnSaveButtonClick()
    {
        // Get the player's name from the input field
        string playerName = playerNameInputField.text;

        // Check if the name is not empty
        if (!string.IsNullOrEmpty(playerName))
        {
            // Save the player name (Display it or store it in player preferences)
            playerNameDisplayText.text = playerName;
            selectedImageDisplay.sprite = ProfileManager.profile_Instance.profileImage.sprite;
            Debug.Log("Image change");

            // Hide the player profile panel
            playerProfilePanel.SetActive(false);
            Debug.Log("Player profile panel hidden.");

            // Show the home screen panel
            homeScreenPanel.SetActive(true);
            Debug.Log("Home screen panel should now be visible.");

            // Check if the panel is actually active
            Debug.Log("Is homeScreenPanel active? " + homeScreenPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Player name is empty, please enter a name!");
        }
    }

}
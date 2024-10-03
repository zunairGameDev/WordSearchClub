using UnityEngine;
using UnityEngine.UI;

public class PanelManager1 : MonoBehaviour
{
    public GameObject mainPanel;       // Original panel with 3 buttons
    public GameObject panel1;          // Panel shown when button 1 is clicked
    public GameObject panel2;          // Panel shown when button 2 is clicked
    public GameObject panel3;          // Panel shown when button 3 is clicked
    public Button button1;
    public Button button2;
    public Button button3;
    public Button backButton;          // Back button to return to main panel
    public Text unlockMessage;         // Text to show unlock message for locked buttons

    private int currentLevel = 5;      // For testing, change this to actual level tracking
    private int unlockLevel = 10;      // Level required to unlock button2 and button3

    void Start()
    {
        // Add button listeners
        button1.onClick.AddListener(ShowPanel1);
        button2.onClick.AddListener(ShowPanel2);
        button3.onClick.AddListener(ShowPanel3);
        backButton.onClick.AddListener(ShowMainPanel);

        ShowMainPanel();  // Initially show the main panel
    }

    void ShowMainPanel()
    {
        // Activate the main panel and deactivate other panels
        mainPanel.SetActive(true);
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(false);
        unlockMessage.gameObject.SetActive(false);  // Hide the unlock message
    }

    void ShowPanel1()
    {
        // Show panel 1 and hide the main panel
        mainPanel.SetActive(false);
        panel1.SetActive(true);
    }

    void ShowPanel2()
    {
        if (currentLevel >= unlockLevel)
        {
            // Show panel 2 if unlocked
            mainPanel.SetActive(false);
            panel2.SetActive(true);
        }
        else
        {
            // Show unlock message if not unlocked
            unlockMessage.text = "This button will unlock after 10 levels.";
            unlockMessage.gameObject.SetActive(true);
        }
    }

    void ShowPanel3()
    {
        if (currentLevel >= unlockLevel)
        {
            // Show panel 3 if unlocked
            mainPanel.SetActive(false);
            panel3.SetActive(true);
        }
        else
        {
            // Show unlock message if not unlocked
            unlockMessage.text = "This button will unlock after 10 levels.";
            unlockMessage.gameObject.SetActive(true);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : UIScreenBase
{
    [SerializeField] private Button startButton;
    [SerializeField] private Text startButtonText;

    public Button GetStartButton()
    {
        return startButton;
    }
    public void SetStartButtonText(string levelText)
    {
        startButtonText.text = levelText;
    }
}

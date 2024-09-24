using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteScreen : UIScreenBase
{
    [SerializeField] private Button nextLevelButton;

    public Button GetNextLevelButton()
    {
        return nextLevelButton;
    }
}

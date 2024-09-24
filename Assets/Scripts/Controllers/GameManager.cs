using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region dependencies
    private GameplayController _gameplayController;
    private LevelManager _levelManager;
    private UIManager _uiManager;
    #endregion

    #region events
    public Action onLevelStarted;
    public Action<int> onLevelCompleted;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAndInstantiateControllers();
        _uiManager.SetStartButtonText(_levelManager.GetCurrentLevelName().ToUpper());
    }

    public void StartGame()
    {
        int currentLevel = SaveLoadManager.LoadLevel();
        int rows = _levelManager.GetRowSize(currentLevel);
        int columns = _levelManager.GetColumnSize(currentLevel);

        _gameplayController.SetGridSize(rows, columns);
        _gameplayController.SetWordSource(_levelManager.GetLevelDataBasedOnDifficulty(currentLevel));

        _gameplayController.Setup(currentLevel);
    }

    public void PlayLevel(string sceneName = "GameScene")
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LevelCompleted()
    {
        int levelNumber;
        if (_levelManager.HasMoreLevels())
        {
            levelNumber = SaveLoadManager.LoadLevel();
            levelNumber++;
            SaveLoadManager.SaveLevel(levelNumber);
        }
        else
        {
            levelNumber = 0;
            SaveLoadManager.SaveLevel(levelNumber);
        }
        onLevelCompleted?.Invoke(levelNumber);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void BackToPreviousScene()
    {
        // Example implementation. Modify based on how you handle scene history.
        SceneManager.LoadScene("MainMenu"); // Change "MainMenu" to your actual main menu scene name
    }

    private void LoadAndInstantiateControllers()
    {
        GameplayController gameplayControllerRef = Resources.Load<GameplayController>(Paths.GAMEPLAY_CONTROLLER);
        LevelManager levelManagerRef = Resources.Load<LevelManager>(Paths.LEVEL_MANAGER);
        UIManager uiManagerRef = Resources.Load<UIManager>(Paths.UI_CANVAS);

        if (gameplayControllerRef == null || levelManagerRef == null || uiManagerRef == null)
        {
            Debug.LogError("One or more controller references could not be loaded.");
            return;
        }

        _gameplayController = Instantiate(gameplayControllerRef);
        _levelManager = Instantiate(levelManagerRef);
        _uiManager = Instantiate(uiManagerRef);

        _gameplayController.InjectDependencies(_uiManager);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}

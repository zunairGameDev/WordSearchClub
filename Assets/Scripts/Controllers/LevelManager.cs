using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private TextAsset[] levelsData;
    [SerializeField] private List<LevelSO> levelSOList;

    private List<Level> _levels;
    private int currentlevelIndex;

    private void Awake()
    {
        currentlevelIndex = SaveLoadManager.LoadLevel();
        _levels = new List<Level>();
        PopulateLevelsList();
    }

    public TextAsset GetLevelData(int levelNumber)
    {
        return levelsData[levelNumber];
    }

    public TextAsset GetLevelDataBasedOnDifficulty(int levelNumber)
    {
        WordLibraryType type = _levels[levelNumber].GetLibraryType();
        TextAsset textAsset;
        switch (type)
        {
            case WordLibraryType.begineer:
                textAsset = Resources.Load<TextAsset>(Constants.BEGINEER_LIBRARY);
                break;
            case WordLibraryType.easy:
                textAsset = Resources.Load<TextAsset>(Constants.EASY_LIBRARY);
                break;
            case WordLibraryType.intermediate:
                textAsset = Resources.Load<TextAsset>(Constants.INTERMEDIATE_LIBRARY);
                break;
            default:
                textAsset = Resources.Load<TextAsset>(Constants.EASY_LIBRARY);
                break;
        }
        return textAsset;
    }

    public int GetColumnSize(int levelNumber)
    {
        return _levels[levelNumber].GetColumnSize();
    }

    public int GetRowSize(int levelNumber)
    {
        return _levels[levelNumber].GetRowSize();
    }

    public int GetCurrentLevel()
    {
        return currentlevelIndex;
    }

    public bool HasMoreLevels()
    {
        if (currentlevelIndex < _levels.Count - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetCurrentLevelName()
    {
        return _levels[currentlevelIndex].GetName();
    }

    private void PopulateLevelsList()
    {
        for (int i = 0; i < levelSOList.Count; i++)
        {
            Level temperoryLevel = new Level(levelSOList[i]);
            _levels.Add(temperoryLevel);
        }
    }
}

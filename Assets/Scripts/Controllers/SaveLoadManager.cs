using UnityEngine;

public class SaveLoadManager
{
    public static void SaveLevel(int levelNumber)
    {
        PlayerPrefs.SetInt(Constants.LEVEL_NUMBER, levelNumber);
    }
    public static int LoadLevel()
    {
        int defaultLevelNumber = 0;
        int currentLevelNumber = PlayerPrefs.GetInt(Constants.LEVEL_NUMBER, defaultLevelNumber);
        return currentLevelNumber;
    }
}

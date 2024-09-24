using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "WordSearchPrototype/Level")]
public class LevelSO : ScriptableObject
{
    public string levelName;
    public int columns;
    public int rows;
    public WordLibraryType libraryType;
}

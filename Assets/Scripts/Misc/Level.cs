public class Level
{
    private string _levelName;
    private int _columns;
    private int _rows;
    private WordLibraryType _libraryType;

    public Level(LevelSO levelSO)
    {
        _levelName = levelSO.levelName;
        _columns = levelSO.columns;
        _rows = levelSO.rows;
        _libraryType = levelSO.libraryType;
    }

    public WordLibraryType GetLibraryType()
    {
        return _libraryType;
    }

    public string GetName()
    {
        return _levelName;
    }

    public int GetRowSize()
    {
        return _rows;
    }

    public int GetColumnSize()
    {
        return _columns;
    }
}

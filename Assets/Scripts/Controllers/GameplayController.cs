using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class GameplayController : MonoBehaviour
{
    #region singleton
    public static GameplayController Instance;
    #endregion

    #region dependencies
    private UIManager _uiManager;
    #endregion

    #region events
    public static Action<RectTransform, RectTransform> FoundWord;
    public static Action<List<RectTransform>> SelectingWord;
    public static Action Finish;
    public static Action ClearSelectingLine;
    public static Action LineColorSelection;
    public static Action<float> Line_Thickness;
    #endregion


    private string[,] _lettersGrid;
    private Transform[,] _lettersTransforms;
    private string _alphabet = "abcdefghijklmnopqrstuvwxyz";
    private int _totalWordsCount;
    private List<RectTransform> selectingWordReactTransform = new List<RectTransform>();

    [Header("Settings")]
    [SerializeField] private bool invertedWordsAreValid;

    private TextAsset _wordsSource;

    [Space]

    [Header("List of Words")]
    public List<string> words = new List<string>();
    public List<string> insertedWords = new List<string>();
    [Header("Grid Settings")]
    public Vector2 gridSize;
    [Space]

    [Header("Cell Settings")]
    public Vector2 cellSize;
    public Vector2 cellSpacing;
    [Space]

    [Header("Public References")]
    public GameObject letterPrefab;
    [Space]

    [Header("Game Detection")]
    public string word;
    public Vector2 origin;
    public Vector2 direction;
    public bool activated;

    [HideInInspector]
    public List<Transform> highlightedObjects = new List<Transform>();

    public int wordCounter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InjectDependencies(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    public void Setup(int levelNo)
    {
        PrepareWords(levelNo);
        InitializeGrid();
        InsertWordsOnGrid();
        RandomizeEmptyCells();
        DisplaySelectedWords();
    }

    public void SetGridSize(int rowSize, int columnSize)
    {
        int minRowSize = 5;
        int maxRowSize = 6;
        int minColumnSize = 5;
        int maxColumnSize = 6;

        rowSize = Mathf.Clamp(rowSize, minRowSize, maxRowSize);
        columnSize = Mathf.Clamp(columnSize, minColumnSize, maxColumnSize);
        gridSize = new Vector2(rowSize, columnSize);
    }

    public void LetterClick(int x, int y, bool state)
    {
        activated = state;
        origin = state ? new Vector2(x, y) : origin;
        direction = state ? direction : new Vector2(-1, -1);

        if (!state)
        {
            ClearSelectingLine();
            ValidateWord();
        }
        else
        {
            LineColorSelection();
        }
    }

    public void LetterHover(int x, int y)
    {
        if (activated)
        {
            direction = new Vector2(x, y);
            if (IsLetterAligned(x, y))
            {
                HighlightSelectedLetters(x, y);
            }
        }
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public bool IsLetterAligned(int x, int y)
    {
        return (origin.x == x || origin.y == y || Math.Abs(origin.x - x) == Math.Abs(origin.y - y));
    }

    public void SetWordSource(TextAsset wordSource)
    {
        _wordsSource = wordSource;
    }

    private void PrepareWords(int levelNo)
    {
        words = _wordsSource.text.Split(',').ToList();

        for (int i = 0; i < words.Count; i++)
        {
            string temp = words[i];

            System.Random rn = new System.Random();

            int randomIndex = rn.Next(words.Count());
            words[i] = words[randomIndex];
            words[randomIndex] = temp;
        }

        int maxGridDimension = Mathf.Max((int)gridSize.x, (int)gridSize.y);

        words = words.Where(x => x.Length <= maxGridDimension).ToList();
    }

    private void InitializeGrid()
    {
        _lettersGrid = new string[(int)gridSize.x, (int)gridSize.y];
        _lettersTransforms = new Transform[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                _lettersGrid[x, y] = "";

                GameObject letter = Instantiate(letterPrefab, _uiManager.GetGridTransform());

                letter.name = x.ToString() + "-" + y.ToString();

                _lettersTransforms[x, y] = letter.transform;
            }
        }
        ApplyGridSettings();
    }

    private void ApplyGridSettings()
    {
        GridLayoutGroup gridLayout = _uiManager.GetGridLayoutGroup();

        gridLayout.cellSize = cellSize;
        gridLayout.spacing = cellSpacing;

        int cellSizeX = (int)gridLayout.cellSize.x + (int)gridLayout.spacing.x;
        int cellSizeY = (int)gridLayout.cellSize.y + (int)gridLayout.spacing.y;
        _uiManager.GetGridRectTransform().sizeDelta = new Vector2(cellSizeX * gridSize.x, 0);

        // offset add to differentiate between grid and its parent image
        Vector2 offset = new Vector2(50f, 50f);
        _uiManager.GetGridRectTransform().parent.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSizeY * gridSize.y, cellSizeX * gridSize.y) + offset;
    }

    private void InsertWordsOnGrid()
    {
        foreach (string word in words)
        {
            System.Random rn = new System.Random();

            bool inserted = false;
            int tryAmount = 0;
            do
            {
                int row = rn.Next((int)gridSize.x);
                int column = rn.Next((int)gridSize.y);

                int dirX = 0; int dirY = 0;

                while (dirX == 0 && dirY == 0)
                {
                    if (invertedWordsAreValid)
                    {
                        dirX = rn.Next(3) - 1;
                        dirY = rn.Next(3) - 1;
                    }
                    else
                    {
                        dirX = rn.Next(2);
                        dirY = rn.Next(2);
                    }
                }

                inserted = InsertWord(word, row, column, dirX, dirY);
                tryAmount++;

            } while (!inserted && tryAmount < 100);

            if (inserted)
                insertedWords.Add(word);
        }
        _totalWordsCount = insertedWords.Count;
    }

    private bool InsertWord(string word, int row, int column, int dirX, int dirY)
    {

        if (!CanInsertWordOnGrid(word, row, column, dirX, dirY))
            return false;

        for (int i = 0; i < word.Length; i++)
        {
            _lettersGrid[(i * dirX) + row, (i * dirY) + column] = word[i].ToString();
            Transform t = _lettersTransforms[(i * dirX) + row, (i * dirY) + column];
            t.GetComponentInChildren<TextMeshProUGUI>().text = word[i].ToString().ToUpper();
        }
        LineThickness(_uiManager.GetGridTransform().GetChild(0).GetComponentInChildren<TextMeshProUGUI>().fontSize);
        return true;
    }

    private bool CanInsertWordOnGrid(string word, int row, int column, int dirX, int dirY)
    {
        if (dirX > 0)
        {
            if (row + word.Length > gridSize.x)
            {
                return false;
            }
        }
        if (dirX < 0)
        {
            if (row - word.Length < 0)
            {
                return false;
            }
        }
        if (dirY > 0)
        {
            if (column + word.Length > gridSize.y)
            {
                return false;
            }
        }
        if (dirY < 0)
        {
            if (column - word.Length < 0)
            {
                return false;
            }
        }

        for (int i = 0; i < word.Length; i++)
        {
            string currentCharOnGrid = (_lettersGrid[(i * dirX) + row, (i * dirY) + column]);
            string currentCharOnWord = (word[i].ToString());

            if (currentCharOnGrid != String.Empty && currentCharOnWord != currentCharOnGrid)
            {
                return false;
            }
        }

        return true;
    }

    private void LineThickness(float lineThicknessValue)
    {
        Line_Thickness(lineThicknessValue);
    }

    private void RandomizeEmptyCells()
    {
        System.Random rn = new System.Random();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_lettersGrid[x, y] == string.Empty)
                {
                    _lettersGrid[x, y] = _alphabet[rn.Next(_alphabet.Length)].ToString();
                    _lettersTransforms[x, y].GetComponentInChildren<TextMeshProUGUI>().text = _lettersGrid[x, y].ToUpper();
                }
            }
        }
    }

    private void ValidateWord()
    {
        word = string.Empty;

        foreach (Transform t in highlightedObjects)
        {
            word += t.GetComponentInChildren<TextMeshProUGUI>().text.ToLower();
        }

        if (insertedWords.Contains(word) || insertedWords.Contains(Reverse(word)))
        {
            //foreach (Transform h in highlightedObjects)
            //{
            //    h.GetComponent<Image>().color = Color.white;
            //    h.transform.DOPunchScale(-Vector3.one, 0.2f, 10, 1);
            //}

            //Visual Event
            RectTransform r1 = highlightedObjects[0].GetComponent<RectTransform>();
            RectTransform r2 = highlightedObjects[highlightedObjects.Count() - 1].GetComponent<RectTransform>();
            FoundWord(r1, r2);
            wordCounter++;
            if (wordCounter == _totalWordsCount)
            {
                GameManager.Instance.LevelCompleted();
            }
            Debug.Log("<b>" + word.ToUpper() + "</b> was found!");

            ScrollViewWords.instance.CheckWord(word);

            insertedWords.Remove(word);
            insertedWords.Remove(Reverse(word));

            if (insertedWords.Count <= 0)
            {
                Finish();
            }
        }
        else
        {
            ClearWordSelection();
        }
    }

    private void HighlightSelectedLetters(int x, int y)
    {
        ClearWordSelection();

        Color selectColor = HighlightBehaviour.instance.colors[HighlightBehaviour.instance.colorCounter];

        if (x == origin.x)
        {
            int min = (int)Math.Min(y, origin.y);
            int max = (int)Math.Max(y, origin.y);

            for (int i = min; i <= max; i++)
            {
                //_lettersTransforms[x, i].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(_lettersTransforms[x, i]);
                HighLightSelectingWords(highlightedObjects);
            }
        }
        else if (y == origin.y)
        {
            int min = (int)Math.Min(x, origin.x);
            int max = (int)Math.Max(x, origin.x);

            for (int i = min; i <= max; i++)
            {
                //_lettersTransforms[i, y].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(_lettersTransforms[i, y]);
                HighLightSelectingWords(highlightedObjects);
            }
        }
        else
        {
            int incX = (origin.x > x) ? -1 : 1;
            int incY = (origin.y > y) ? -1 : 1;
            int steps = (int)Math.Abs(origin.x - x);

            for (int i = 0, curX = (int)origin.x, curY = (int)origin.y; i <= steps; i++, curX += incX, curY += incY)
            {
                //_lettersTransforms[curX, curY].GetComponent<Image>().color = selectColor;
                highlightedObjects.Add(_lettersTransforms[curX, curY]);
                HighLightSelectingWords(highlightedObjects);
            }
        }
    }
    private void HighLightSelectingWords(List<Transform> highlightedObjects)
    {
        selectingWordReactTransform.Clear();
        foreach (Transform t in highlightedObjects)
        {
            selectingWordReactTransform.Add(t.GetComponent<RectTransform>());
            SelectingWord(selectingWordReactTransform);
        }
    }

    private void ClearWordSelection()
    {
        //foreach (Transform h in highlightedObjects)
        //{
        //    h.GetComponent<Image>().color = Color.white;
        //}

        highlightedObjects.Clear();

    }

    private void DisplaySelectedWords()
    {
        ScrollViewWords.instance.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        float delay = 0;

        for (int i = 0; i < insertedWords.Count; i++)
        {
            ScrollViewWords.instance.SpawnWordCell(insertedWords[i], delay);
            delay += .05f;
        }
    }

    private void HideSelectedWords()
    {
        ScrollViewWords.instance.gameObject.GetComponent<CanvasGroup>().alpha = 0;
    }

}

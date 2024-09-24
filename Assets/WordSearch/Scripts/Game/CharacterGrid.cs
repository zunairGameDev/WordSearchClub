using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Xml;
//using System.Drawing;

namespace BBG.WordSearch
{
    public class CharacterGrid : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {

        public Color colorTransperancy;
        public Color colorOpque;
        public Vector2 offset;
        public Vector2 startMousePosition;
        public Vector2 currentMousePosition;
        public bool increaseDistanceAllowed = true;
        public float permenetLineDistance = 0;
        public float lastDistance = 0;
        public GameObject permentLineChild;
        public bool pointCheck;

        public List<CharacterGridItem> letterObject;
        public EdgeDeductor deductor;

        #region Enums

        private enum HighlighPosition
        {
            AboveLetters,
            BelowLetters
        }

        #endregion

        #region Inspector Variables

        [SerializeField] private float maxCellSize = 0;
        [SerializeField] private SelectedWord selectedWord = null;

        [Header("Letter Settings")]
        [SerializeField] private Font letterFont = null;
        [SerializeField] private int letterFontSize = 0;
        [SerializeField] private Color letterColor = Color.white;
        [SerializeField] private Color letterHighlightedColor = Color.white;
        [SerializeField] private Vector2 letterOffsetInCell = Vector2.zero;

        [Header("Highlight Settings")]
        [SerializeField] private HighlighPosition highlightPosition = HighlighPosition.AboveLetters;
        [SerializeField] private Sprite highlightSprite = null;
        [SerializeField] private float highlightExtraSize = 0f;
        [SerializeField] private List<Color> highlightColors = null;

        [Header("Highlight Letter Settings")]
        [SerializeField] private Sprite highlightLetterSprite = null;
        [SerializeField] private float highlightLetterSize = 0f;
        [SerializeField] private Color highlightLetterColor = Color.white;

        [Header("GridBackGround")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform backgroundShadow;
        [SerializeField] private GameObject prefabPermanentLine;
        private Vector2 lastValidPosition; // Store the last valid position

        #endregion

        #region Member Variables

        public Board currentBoard;

        private RectTransform gridContainer;
        private RectTransform gridOverlayContainer;
        private RectTransform gridUnderlayContainer;
        private RectTransform highlighLetterContainer;
        private ObjectPool characterPool;
        private ObjectPool highlightLetterPool;
        private List<List<CharacterGridItem>> characterItems;
        private List<Image> highlights;

        private float currentScale;
        private float currentCellSize;

        // Used when the player is selecting a word
        private Image selectingHighlight;
        private Image parmentSelectingHighLight;
        private Image newHighlight;
        private bool isSelecting;
        private int selectingPointerId;
        private CharacterGridItem startCharacter;
        private CharacterGridItem lastEndCharacter;

        #endregion

        #region Properties

        private float ScaledHighlighExtraSize { get { return highlightExtraSize * currentScale; } }
        private Vector2 ScaledLetterOffsetInCell { get { return letterOffsetInCell * currentScale; } }
        private float ScaledHightlightLetterSize { get { return highlightLetterSize * currentScale; } }
        private float CellFullWidth { get { return currentCellSize; } }
        private float CellFullHeight { get { return currentCellSize; } }

        #endregion

        public float currentAngle;

        #region Unity Methods

        public void OnPointerDown(PointerEventData eventData)
        {
            if (selectingPointerId != -1)
            {
                // There is already a mouse/pointer highlighting words 
                return;
            }
            permenetLineDistance = 35;
            if (GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
            {
                // Get the closest word to select
                CharacterGridItem characterItem = GetCharacterItemAtPosition(eventData.position);

                if (characterItem != null)
                {
                    // Start selecting
                    isSelecting = true;
                    selectingPointerId = eventData.pointerId;
                    startCharacter = characterItem;
                    lastEndCharacter = characterItem;
                    //AddLetterForDistance(lastEndCharacter);
                    AssignHighlighColor(permentLineChild.GetComponent<Image>());
                    AssignHighlighColor(selectingHighlight);
                    colorTransperancy = permentLineChild.GetComponent<Image>().color;
                    colorOpque = permentLineChild.GetComponent<Image>().color;
                    selectingHighlight.gameObject.SetActive(true);
                    UpdateSelectingHighlight(eventData.position);
                    UpdateSelectedWord();
                    pointCheck = false;
                    SoundManager.Instance.Play("highlight");
                }

                startMousePosition = eventData.position;
                lastValidPosition = new Vector2(0f, 0f);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != selectingPointerId)
            {
                return;
            }

            if (GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
            {
                if (eventData.pointerId != selectingPointerId)
                {
                    return;
                }

                if (GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
                {

                    // Get mouse position in world space

                    // Constrain the mouse position within the parent rect (background)
                    pointCheck = true;
                    Vector2 mousePosition = GetMousePosition(eventData.position);
                    Vector2 constrainedPosition = ConstrainToRect(mousePosition, background, lastValidPosition);
                    currentMousePosition = constrainedPosition;
                    UpdateSelectingHighlight(eventData.position);
                    UpdateSelectedWord();

                    // Update last valid position if the constrained position has changed
                    //if (constrainedPosition != lastValidPosition)
                    //{
                    //    lastValidPosition = constrainedPosition;
                    //}

                    // Use the constrained mouse position

                    // Debugging logs (optional)
                    //Debug.Log(currentMousePosition + " Constrained Position");
                }
                //Debug.Log(currentMousePosition + " with Contrain");
                //currentMousePosition = eventData.position;
                //Debug.Log(currentMousePosition + "with out Position");
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != selectingPointerId)
            {
                return;
            }
            increaseDistanceAllowed = true;
            if (startCharacter != null && lastEndCharacter != null && GameManager.Instance.ActiveGameState == GameManager.GameState.BoardActive)
            {
                // Set the text color back to the normal color, if the selected word is actually a word then the HighlightWord will set the color back to the highlighted color
                SetTextColor(startCharacter, lastEndCharacter, letterColor, false);

                // Get the start and end row/col position for the word
                Cell wordStartPosition = new Cell(startCharacter.Row, startCharacter.Col);
                Cell wordEndPosition = new Cell(lastEndCharacter.Row, lastEndCharacter.Col);

                string highlightedWord = GetWord(wordStartPosition, wordEndPosition);

                // Call OnWordSelected to notify the WordSearchController that a word has been selected
                string foundWord = GameManager.Instance.OnWordSelected(highlightedWord);
                pointCheck = false;
                // If the word was a word that was suppose to be found then highligh the word and create the floating text
                if (!string.IsNullOrEmpty(foundWord))
                {
                    ShowWord(wordStartPosition, wordEndPosition, foundWord, true);

                    SoundManager.Instance.Play("word-found");
                }
            }

            // End selecting and hide the select highlight
            isSelecting = false;
            selectingPointerId = -1;
            startCharacter = null;
            lastEndCharacter = null;
            selectingHighlight.gameObject.SetActive(false);
            parmentSelectingHighLight.gameObject.SetActive(false);
            selectedWord.Clear();
            letterObject.Clear();
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            // In order for the IPointer/IDrag handlers to work properly we need to put a graphic component this gameobject
            if (gameObject.GetComponent<Graphic>() == null)
            {
                Image image = gameObject.AddComponent<Image>();
                image.color = Color.clear;
            }
            // Create a container that will hold the letter images whena highlight letter hint is used
            highlighLetterContainer = CreateContainer("highligh_letter_container", typeof(RectTransform));
            // Create a GameObject to hold all the letters, set it as a child of CharacterGrid and set its anchors to expand to fill
            gridContainer = CreateContainer("grid_container", typeof(RectTransform), typeof(GridLayoutGroup), typeof(CanvasGroup));

            // Create a GameObject that will be be used to place things overtop of the letter grid
            gridOverlayContainer = CreateContainer("grid_overlay_container", typeof(RectTransform));
            GetComponent<ScaleAndRotate>().cellParent = gridContainer;
            // Only need an underlay container if the higlighs position is set to under the letters
            if (highlightPosition == HighlighPosition.BelowLetters)
            {
                // Create a GameObject that will be be used to place things under the letter grid
                gridUnderlayContainer = CreateContainer("grid_underlay_container", typeof(RectTransform));

                gridUnderlayContainer.SetAsFirstSibling();
            }


            // Create a CharacterGridItem that will be used as a template by the ObjectPool to create more instance
            CharacterGridItem templateCharacterGridItem = CreateCharacterGridItem();
            templateCharacterGridItem.name = "template_character_grid_item";
            templateCharacterGridItem.gameObject.SetActive(false);
            templateCharacterGridItem.transform.SetParent(transform, false);

            GameObject characterPoolContainer = new GameObject("character_pool");
            characterPoolContainer.transform.SetParent(transform);
            characterPoolContainer.SetActive(false);

            // Create a highlight letter image that will be used as a template by the ObjectPool to create more instance
            Image templateHighlightLetterImage = CreateHighlightLetterImage();
            templateCharacterGridItem.name = "template_highlight_letter_image";
            templateCharacterGridItem.gameObject.SetActive(false);
            templateCharacterGridItem.transform.SetParent(transform, false);

            highlightLetterPool = new ObjectPool(templateHighlightLetterImage.gameObject, 1, highlighLetterContainer);
            characterPool = new ObjectPool(templateCharacterGridItem.gameObject, 25, characterPoolContainer.transform);
            characterItems = new List<List<CharacterGridItem>>();
            highlights = new List<Image>();

            // Instantiate an instance of the highlight to use when the player is selecting a word
            selectingHighlight = CreateNewHighlight(true);
            parmentSelectingHighLight = CreateParmentHighlight();

            newHighlight = CreateNewPermanentHighlight();
            selectingHighlight.gameObject.SetActive(false);
            parmentSelectingHighLight.gameObject.SetActive(false);
            selectingPointerId = -1;
        }

        public void Setup(Board board)
        {
            Clear();

            // We want to scale the CharacterItem so that the UI Text changes size
            currentCellSize = SetupGridContainer(board.rows, board.cols);
            currentScale = currentCellSize / maxCellSize;

            for (int i = 0; i < board.boardCharacters.Count; i++)
            {
                characterItems.Add(new List<CharacterGridItem>());

                for (int j = 0; j < board.boardCharacters[i].Count; j++)
                {
                    // Get a new character from the object pool
                    CharacterGridItem characterItem = characterPool.GetObject().GetComponent<CharacterGridItem>();

                    characterItem.Row = i;
                    characterItem.Col = j;
                    characterItem.IsHighlighted = false;

                    characterItem.gameObject.SetActive(true);
                    characterItem.transform.SetParent(gridContainer, false);

                    characterItem.characterText.text = board.boardCharacters[i][j].ToString();
                    characterItem.characterText.color = letterColor;
                    characterItem.characterText.transform.localScale = new Vector3(currentScale, currentScale, 1f);
                    if (characterItem.gameObject.GetComponent<EdgeDeductor>() == null)
                    {
                        characterItem.gameObject.AddComponent<EdgeDeductor>();
                    }
                    else
                    {
                        DestroyImmediate(characterItem.gameObject.GetComponent<EdgeDeductor>());
                        characterItem.gameObject.AddComponent<EdgeDeductor>();
                    }
                    if (characterItem.Row == 0 || characterItem.Row == board.rows - 1 || characterItem.Col == 0 || characterItem.Col == board.cols - 1)
                    {
                        characterItem.gameObject.GetComponent<EdgeDeductor>().isEdge = true;
                    }
                    characterItem.gameObject.GetComponent<EdgeDeductor>().characterGrid = this;
                    (characterItem.characterText.transform as RectTransform).anchoredPosition = ScaledLetterOffsetInCell;

                    characterItems[i].Add(characterItem);
                }
            }

            currentBoard = board;

            UIAnimation anim = UIAnimation.Alpha(gridContainer.GetComponent<CanvasGroup>(), 0f, 1f, .5f);
            anim.style = UIAnimation.Style.EaseOut;
            anim.Play();
        }
        public void GridBackGroundSizeSetting(float newValue)
        {
            // Adjust both offsetMin and offsetMax using the same newValue
            background.offsetMin = new Vector2(newValue, newValue);  // Adjust left and bottom
            background.offsetMax = new Vector2(-newValue, -newValue);  // Adjust right and top
            backgroundShadow.offsetMin = new Vector2(newValue, newValue);  // Adjust left and bottom
            backgroundShadow.offsetMax = new Vector2(-newValue, -newValue);  // Adjust right and top
        }

        public Image HighlightWord(Cell start, Cell end, bool useSelectedColour)
        {
            Image highlight = CreateNewHighlight(false);
            //AssignHighlighColor(highlight);
            highlights.Add(highlight);

            CharacterGridItem startCharacterItem = characterItems[start.row][start.col];
            CharacterGridItem endCharacterItem = characterItems[end.row][end.col];

            // Position the highlight over the letters
            PositionHighlight(highlight, startCharacterItem, endCharacterItem);

            // Set the text color of the letters to the highlighted color
            SetTextColor(startCharacterItem, endCharacterItem, letterHighlightedColor, true);

            if (useSelectedColour && selectingHighlight != null)
            {
                highlight.color = permentLineChild.GetComponent<Image>().color;
                highlight.color = colorOpque;
            }

            return highlight;
        }

        public void SetWordFound(string word)
        {
            if (currentBoard == null)
            {
                return;
            }

            for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
            {
                Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];

                if (word == wordPlacement.word)
                {
                    Cell startPosition = wordPlacement.startingPosition;
                    Cell endPosition = new Cell(startPosition.row + wordPlacement.verticalDirection * (word.Length - 1), startPosition.col + wordPlacement.horizontalDirection * (word.Length - 1));

                    HighlightWord(startPosition, endPosition, false);

                    break;
                }
            }
        }

        public void Clear()
        {
            characterPool.ReturnAllObjectsToPool();
            highlightLetterPool.ReturnAllObjectsToPool();
            characterItems.Clear();

            for (int i = 0; i < highlights.Count; i++)
            {
                Destroy(highlights[i].gameObject);
            }

            highlights.Clear();

            gridContainer.GetComponent<CanvasGroup>().alpha = 0f;
        }

        public void AddLetterForDistance(CharacterGridItem letterItem)
        {
            if (!letterObject.Contains(letterItem))
            {
                letterObject.Add(letterItem);
            }
        }

        public void ShowWordHint(string word)
        {
            if (currentBoard == null)
            {
                return;
            }

            for (int i = 0; i < currentBoard.wordPlacements.Count; i++)
            {
                Board.WordPlacement wordPlacement = currentBoard.wordPlacements[i];

                if (word == wordPlacement.word)
                {
                    Cell startPosition = wordPlacement.startingPosition;
                    Cell endPosition = new Cell(startPosition.row + wordPlacement.verticalDirection * (word.Length - 1), startPosition.col + wordPlacement.horizontalDirection * (word.Length - 1));

                    ShowWord(startPosition, endPosition, word, false);

                    break;
                }
            }
        }

        public void ShowLetterHint(char letterToShow)
        {
            for (int row = 0; row < currentBoard.rows; row++)
            {
                for (int col = 0; col < currentBoard.cols; col++)
                {
                    char letter = currentBoard.boardCharacters[row][col];

                    if (letter == letterToShow)
                    {
                        CharacterGridItem characterGridItem = characterItems[row][col];

                        Vector2 position = (characterGridItem.transform as RectTransform).anchoredPosition;

                        RectTransform highlightLetter = highlightLetterPool.GetObject<RectTransform>();

                        highlightLetter.sizeDelta = new Vector2(ScaledHightlightLetterSize, ScaledHightlightLetterSize);

                        highlightLetter.anchoredPosition = position;
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private RectTransform CreateContainer(string name, params System.Type[] types)
        {
            GameObject containerObj = new GameObject(name, types);
            RectTransform container = containerObj.GetComponent<RectTransform>();

            container.SetParent(transform, false);
            container.anchoredPosition = Vector2.zero;
            container.anchorMin = Vector2.zero;
            container.anchorMax = Vector2.one;
            container.offsetMin = Vector2.zero;
            container.offsetMax = Vector2.zero;

            return container;
        }

        private void ShowWord(Cell wordStartPosition, Cell wordEndPosition, string word, bool useSelectedColor)
        {

            CharacterGridItem startCharacter = characterItems[wordStartPosition.row][wordStartPosition.col];
            CharacterGridItem endCharacter = characterItems[wordEndPosition.row][wordEndPosition.col];

            Image highlight = HighlightWord(wordStartPosition, wordEndPosition, useSelectedColor);
            ScaleGameObject(highlight.transform);
            // Create the floating text in the middle of the highlighted word
            Vector2 startPosition = (startCharacter.transform as RectTransform).anchoredPosition;
            Vector2 endPosition = (endCharacter.transform as RectTransform).anchoredPosition;
            Vector2 center = endPosition + (startPosition - endPosition) / 2f;

            Text floatingText = CreateFloatingText(word, highlight.color, center);

            Color toColor = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, 0f);

            UIAnimation anim;

            anim = UIAnimation.PositionY(floatingText.rectTransform, center.y, center.y + 75f, 1f);
            anim.Play();

            anim = UIAnimation.Color(floatingText, toColor, 1f);
            anim.OnAnimationFinished = (GameObject obj) => { GameObject.Destroy(obj); };
            anim.Play();
        }

        public void ScaleGameObject(Transform image)
        {
            float scaleIncrease = 0.15f;  // Amount to increase the scale
            float scaleDuration = 0.2f;  // Duration of the scaling animation
            float waitTime = 0.1f;       // Time to wait before returning to original scale

            // Store the original scale (only X and Y are relevant for 2D)
            Vector3 originalScale = image.localScale;

            // Increase the scale by 0.3 on X and Y axes (keeping Z the same for 2D objects)
            Vector3 increasedScale = new Vector3(originalScale.x * (1 + scaleIncrease), originalScale.y * (1 + scaleIncrease), originalScale.z);

            // Scale up to the increased scale, then wait and return to the original size
            image.DOScale(increasedScale, scaleDuration)
                .OnComplete(() =>
                {
                    // After a delay, return the object to its original scale
                    image.DOScale(originalScale, scaleDuration).SetDelay(waitTime);
                });
        }

        private string GetWord(Cell start, Cell end)
        {
            int rowInc = (start.row == end.row) ? 0 : (start.row < end.row ? 1 : -1);
            int colInc = (start.col == end.col) ? 0 : (start.col < end.col ? 1 : -1);
            int incAmount = Mathf.Max(Mathf.Abs(start.row - end.row), Mathf.Abs(start.col - end.col));

            string word = "";

            for (int i = 0; i <= incAmount; i++)
            {
                word = word + currentBoard.boardCharacters[start.row + i * rowInc][start.col + i * colInc];
            }

            return word;
        }

        private void UpdateSelectedWord()
        {
            if (startCharacter != null && lastEndCharacter != null)
            {
                Cell wordStartPosition = new Cell(startCharacter.Row, startCharacter.Col);
                Cell wordEndPosition = new Cell(lastEndCharacter.Row, lastEndCharacter.Col);

                selectedWord.SetSelectedWord(GetWord(wordStartPosition, wordEndPosition), selectingHighlight.color);
            }
            else
            {
                selectedWord.Clear();
            }
        }

        private void UpdateSelectingHighlight(Vector2 screenPosition)
        {
            if (isSelecting)
            {
                CharacterGridItem endCharacter = GetCharacterItemAtPosition(screenPosition);

                // If endCharacter is null then the mouse position must be off the grid container
                if (endCharacter != null)
                {
                    int startRow = startCharacter.Row;
                    int startCol = startCharacter.Col;

                    int endRow = endCharacter.Row;
                    int endCol = endCharacter.Col;

                    int rowDiff = endRow - startRow;
                    int colDiff = endCol - startCol;

                    // Check to see if the line from the start to the end is not vertical/horizontal/diagonal
                    if (rowDiff != colDiff && rowDiff != 0 && colDiff != 0)
                    {
                        // Now we will find the best new end character position. All code below makes the highlight snap to a proper vertical/horizontal/diagonal line
                        if (Mathf.Abs(colDiff) > Mathf.Abs(rowDiff))
                        {
                            if (Mathf.Abs(colDiff) - Mathf.Abs(rowDiff) > Mathf.Abs(rowDiff))
                            {
                                rowDiff = 0;
                            }
                            else
                            {
                                colDiff = AssignKeepSign(colDiff, rowDiff);
                            }
                        }
                        else
                        {
                            if (Mathf.Abs(rowDiff) - Mathf.Abs(colDiff) > Mathf.Abs(colDiff))
                            {
                                colDiff = 0;
                            }
                            else
                            {
                                colDiff = AssignKeepSign(colDiff, rowDiff);
                            }
                        }

                        if (startCol + colDiff < 0)
                        {
                            colDiff = colDiff - (startCol + colDiff);
                            rowDiff = AssignKeepSign(rowDiff, Mathf.Abs(colDiff));
                        }
                        else if (startCol + colDiff >= currentBoard.cols)
                        {
                            colDiff = colDiff - (startCol + colDiff - currentBoard.cols + 1);
                            rowDiff = AssignKeepSign(rowDiff, Mathf.Abs(colDiff));
                        }

                        endCharacter = characterItems[startRow + rowDiff][startCol + colDiff];
                    }
                }
                else
                {
                    // Use the last selected end character
                    endCharacter = lastEndCharacter;
                    //Debug.Log("A");
                }

                if (lastEndCharacter != null)
                {
                    SetTextColor(startCharacter, lastEndCharacter, letterColor, false);
                }

                // Position the select highlight in the proper position
                PositionHighlight(selectingHighlight, startCharacter, endCharacter);
                if (pointCheck)
                {
                    permentHighLight(parmentSelectingHighLight, startCharacter, currentAngle, GetMousePosition(screenPosition));

                }
                //permentHighLight(newHighlight, startCharacter, currentAngle, screenPosition);
                // Set the text color of the letters to the highlighted color
                SetTextColor(startCharacter, endCharacter, letterHighlightedColor, false);

                // If the new end character is different then the last play a sound
                if (lastEndCharacter != endCharacter)
                {
                    SoundManager.Instance.Play("highlight");
                }

                // Set the last end character so if the player drags outside the grid container then we have somewhere to drag to
                lastEndCharacter = endCharacter;
                AddLetterForDistance(lastEndCharacter);
            }
        }

        private void PositionHighlight(Image highlight, CharacterGridItem start, CharacterGridItem end)
        {
            //Debug.Log("...");

            RectTransform highlightRectT = highlight.transform as RectTransform;
            Vector2 startPosition = (start.transform as RectTransform).anchoredPosition;
            Vector2 endPosition = (end.transform as RectTransform).anchoredPosition;

            float distance = Vector2.Distance(startPosition, endPosition);
            float highlightWidth = currentCellSize + distance + ScaledHighlighExtraSize;
            float highlightHeight = currentCellSize + ScaledHighlighExtraSize;
            float scale = highlightHeight / highlight.sprite.rect.height;

            // Set position and size
            highlightRectT.anchoredPosition = startPosition + (endPosition - startPosition) / 2f;

            // Now Set the size of the highlight
            highlightRectT.localScale = new Vector3(scale, scale);
            highlightRectT.sizeDelta = new Vector2(highlightWidth / scale, highlight.sprite.rect.height);

            // Set angle
            float angle = Vector2.Angle(new Vector2(1f, 0f), endPosition - startPosition);

            if (startPosition.y > endPosition.y)
            {
                angle = -angle;
            }
            currentAngle = angle;
            highlightRectT.eulerAngles = new Vector3(0f, 0f, angle);
            //Debug.Log(highlightRectT.eulerAngles);
        }
        private void permentHighLight(Image highlight, CharacterGridItem start, float currentAngle, Vector2 mousePositionStart)
        {
            permenetLineDistance = 0;
            //highlight.transform.GetChild(0).GetComponent<Image>().color = selectingHighlight.color;
            highlight.gameObject.SetActive(true);

            RectTransform highlightRectT = highlight.transform as RectTransform;
            RectTransform parentRect = background.transform as RectTransform; // Assuming background is the parent

            Vector2 startPosition = (start.transform as RectTransform).anchoredPosition;

            // Set the start position for the highlight
            highlightRectT.anchoredPosition = startPosition;

            // Constrain the currentMousePosition to the parentRect bounds
            //Vector2 clampedMousePosition = ConstrainToRect(currentMousePosition, parentRect);


            float tempDistance = Vector2.Distance(GetMousePosition(startMousePosition), currentMousePosition);
            Debug.Log(GetMousePosition(startMousePosition) + "Start point");
            Debug.Log(GetMousePosition(currentMousePosition) + "Start point");

            colorTransperancy.a = 0f;

            if (tempDistance > lastDistance)
            {
                if (increaseDistanceAllowed)
                {
                    permentLineChild.GetComponent<Image>().color = colorOpque;
                    selectingHighlight.color = colorTransperancy;
                    permenetLineDistance = Vector2.Distance(GetMousePosition(startMousePosition), currentMousePosition);
                    //permenetLineDistance -= 55;
                    lastDistance = permenetLineDistance;
                }
                else
                {
                    //selectingHighlight.color = permentLineChild.GetComponent<Image>().color;
                    selectingHighlight.color = colorOpque;
                    permentLineChild.GetComponent<Image>().color = colorTransperancy;
                    //permenetLineDistance = lastDistance;
                }
            }
            else
            {
                permentLineChild.GetComponent<Image>().color = colorOpque;
                selectingHighlight.color = colorTransperancy;
                permenetLineDistance = Vector2.Distance(GetMousePosition(startMousePosition), currentMousePosition);
                lastDistance = permenetLineDistance;
            }
            //if (increaseDistanceAllowed)
            //{

            //    permenetLineDistance = Vector2.Distance(GetMousePosition(startMousePosition), currentMousePosition);
            //    permenetLineDistance -= 55;
            //}

            // Calculate the distance between the clamped mouse position and start position

            // Set the size and scale based on distance
            float highlightWidth = currentCellSize + permenetLineDistance + ScaledHighlighExtraSize;
            float highlightHeight = currentCellSize + ScaledHighlighExtraSize;
            float scale = highlightHeight / highlight.sprite.rect.height;

            // Set the size and scale of the highlight
            highlightRectT.localScale = new Vector3(scale, scale);
            highlightRectT.sizeDelta = new Vector2(highlightWidth / scale, highlight.sprite.rect.height); // Adjusting width according to distance

            // Set the rotation angle
            highlight.transform.eulerAngles = new Vector3(0f, 0f, currentAngle);

            //Debug.Log(distance);
        }

        private Vector2 ConstrainToRect(Vector2 position, RectTransform rect, Vector2 lastValidPosition)
        {
            Rect rectBounds = rect.rect;
            float offSet = 100f;  // You can adjust this offset as needed
            Vector2 newPosition = position + new Vector2(100, 100);

            float clampedX = Mathf.Clamp(position.x, rectBounds.xMin + offSet, rectBounds.xMax - offSet);
            float clampedY = Mathf.Clamp(position.y, rectBounds.yMin + offSet, rectBounds.yMax - offSet);


            return new Vector2(clampedX, clampedY);
        }

        /// <summary
        /// Sets all character text colors from start to end 
        /// </summary>
        private void SetTextColor(CharacterGridItem start, CharacterGridItem end, Color color, bool isHighlighted)
        {
            int rowInc = (start.Row == end.Row) ? 0 : (start.Row < end.Row ? 1 : -1);
            int colInc = (start.Col == end.Col) ? 0 : (start.Col < end.Col ? 1 : -1);
            int incAmount = Mathf.Max(Mathf.Abs(start.Row - end.Row), Mathf.Abs(start.Col - end.Col));
            letterObject.Clear();
            for (int i = 0; i <= incAmount; i++)
            {
                CharacterGridItem characterGridItem = characterItems[start.Row + i * rowInc][start.Col + i * colInc];
                AddLetterForDistance(characterGridItem);
                characterGridItem.transform.GetComponent<EdgeDeductor>().CheckingDistance();
                // If the character grid item is part of a word that is highlighed then it's color will always be set to the letterHighlightedColor
                if (characterGridItem.IsHighlighted)
                {
                    characterGridItem.characterText.color = letterHighlightedColor;
                }
                else
                {
                    // If the word is being highlighted then set the flag
                    if (isHighlighted)
                    {
                        characterGridItem.IsHighlighted = isHighlighted;
                    }

                    // Set the text color to the color that was given
                    characterGridItem.characterText.color = color;
                }
            }
        }

        private CharacterGridItem GetCharacterItemAtPosition(Vector2 screenPoint)
        {
            for (int i = 0; i < characterItems.Count; i++)
            {
                for (int j = 0; j < characterItems[i].Count; j++)
                {
                    Vector2 localPoint;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(characterItems[i][j].transform as RectTransform, screenPoint, null, out localPoint);

                    // Check if the localPoint is inside the cell in the grid
                    localPoint.x += CellFullWidth / 2f;
                    localPoint.y += CellFullHeight / 2f;

                    if (localPoint.x >= 0 && localPoint.y >= 0 && localPoint.x < CellFullWidth && localPoint.y < CellFullHeight)
                    {
                        return characterItems[i][j];
                    }
                }
            }

            return null;
        }
        private Vector2 GetMousePosition(Vector2 screenPoint)
        {
            // Convert screen position to local position within the parent RectTransform
            RectTransform rectTransform = background.transform as RectTransform;
            if (rectTransform == null)
            {
                Debug.LogWarning("The transform is not a RectTransform.");
                return Vector2.zero;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                screenPoint,
                null,
                out Vector2 localPoint
                );
            //for (int i = 0; i < characterItems.Count; i++)
            //{
            //    for (int j = 0; j < characterItems[i].Count; j++)
            //    {
            //        Vector2 localPoint;

            //        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, screenPoint, null, out localPoint);

            //        // Check if the localPoint is inside the cell in the grid
            //        localPoint.x += CellFullWidth / 2f;
            //        localPoint.y += CellFullHeight / 2f;

            //        if (localPoint.x >= 0 && localPoint.y >= 0 && localPoint.x < CellFullWidth && localPoint.y < CellFullHeight)
            //        {
            //            return characterItems[i][j];
            //        }
            //    }
            //}

            return localPoint;
        }

        private float SetupGridContainer(int rows, int columns)
        {
            // Add a GridLayoutGroup so make positioning letters much easier
            GridLayoutGroup gridLayoutGroup = gridContainer.GetComponent<GridLayoutGroup>();

            // Get the width and height of a cell
            float cellWidth = gridContainer.rect.width / (float)columns;
            float cellHeight = gridContainer.rect.height / (float)rows;
            float cellSize = Mathf.Min(cellWidth, cellHeight, maxCellSize);

            gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayoutGroup.constraintCount = columns;

            return cellSize;
        }

        private CharacterGridItem CreateCharacterGridItem()
        {
            GameObject characterGridItemObject = new GameObject("character_grid_item", typeof(RectTransform));
            GameObject textObject = new GameObject("character_text", typeof(RectTransform));

            // Set the text object as a child of the CharacterGridItem object and set its position as the offset
            textObject.transform.SetParent(characterGridItemObject.transform);
            (textObject.transform as RectTransform).anchoredPosition = letterOffsetInCell;

            // Add the Text component for the item and set the font/fontSize
            Text characterText = textObject.AddComponent<Text>();
            characterText.font = letterFont;
            characterText.fontSize = letterFontSize;
            characterText.color = letterColor;

            // Create a ContentSizeFitter for the text object so the size will always fit the letter in it
            ContentSizeFitter textCSF = textObject.AddComponent<ContentSizeFitter>();
            textCSF.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            textCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Add the CharacterGridItem component
            CharacterGridItem characterGridItem = characterGridItemObject.AddComponent<CharacterGridItem>();
            characterGridItem.characterText = characterText;

            return characterGridItem;
        }

        private Image CreateHighlightLetterImage()
        {
            GameObject highlightImageObj = new GameObject("highligh_image_obj", typeof(RectTransform));
            Image highlightLetterImage = highlightImageObj.AddComponent<Image>();

            highlightLetterImage.sprite = highlightLetterSprite;
            //highlightLetterImage.color = highlightLetterColor;

            AssignHighlighColor(highlightLetterImage);
            highlightLetterImage.rectTransform.sizeDelta = new Vector2(highlightLetterSize, highlightLetterSize);
            highlightLetterImage.rectTransform.anchorMin = new Vector2(0f, 1f);
            highlightLetterImage.rectTransform.anchorMax = new Vector2(0f, 1f);

            return highlightLetterImage;
        }

        private Image CreateNewHighlight(bool transperanse)
        {
            GameObject highlightObject = new GameObject("highlight");
            RectTransform highlightRectT = highlightObject.AddComponent<RectTransform>();
            Image highlightImage = highlightObject.AddComponent<Image>();

            highlightRectT.anchorMin = new Vector2(0f, 1f);
            highlightRectT.anchorMax = new Vector2(0f, 1f);
            highlightRectT.SetParent(highlightPosition == HighlighPosition.AboveLetters ? gridOverlayContainer : gridUnderlayContainer, false);

            highlightImage.type = Image.Type.Sliced;
            highlightImage.fillCenter = true;
            highlightImage.sprite = highlightSprite;

            AssignHighlighColor(highlightImage);
            if (transperanse)
            {
                highlightImage.color = new Color(0, 0, 0, 0);
            }


            if (selectingHighlight != null)
            {
                // Set the selected highlight as the last sibling so that it will always be drawn ontop of all other highlights
                selectingHighlight.transform.SetAsLastSibling();
            }

            return highlightImage;
        }
        private Image CreateParmentHighlight()
        {
            GameObject highlightObject = new GameObject("Permanenthighlight");
            RectTransform highlightRectT = highlightObject.AddComponent<RectTransform>();
            Image highlightImage = highlightObject.AddComponent<Image>();
            permentLineChild = Instantiate(prefabPermanentLine, highlightObject.transform);
            highlightRectT.anchorMin = new Vector2(0f, 1f);
            highlightRectT.anchorMax = new Vector2(0f, 1f);
            highlightRectT.pivot = new Vector2(0f, 0f);
            highlightRectT.SetParent(highlightPosition == HighlighPosition.AboveLetters ? gridOverlayContainer : gridUnderlayContainer, false);

            highlightImage.type = Image.Type.Sliced;
            highlightImage.fillCenter = true;
            highlightImage.sprite = highlightSprite;
            highlightImage.color = new Color(0, 0, 0, 0);

            //AssignHighlighColor(highlightImage);

            if (selectingHighlight != null)
            {
                // Set the selected highlight as the last sibling so that it will always be drawn ontop of all other highlights
                int secondLastIndex = selectingHighlight.transform.parent.childCount - 2;
                //parmentSelectingHighLight.transform.SetAsLastSibling();
            }

            return highlightImage;
        }
        private Image CreateNewPermanentHighlight()
        {
            GameObject highlightObject = new GameObject("NewHighlight");
            RectTransform highlightRectT = highlightObject.AddComponent<RectTransform>();
            Image highlightImage = highlightObject.AddComponent<Image>();

            //highlightRectT.anchorMin = new Vector2(0f, 1f);
            //highlightRectT.anchorMax = new Vector2(0f, 1f);
            //highlightRectT.pivot = new Vector2(0f, 0f);
            //highlightRectT.SetParent(highlightPosition == HighlighPosition.AboveLetters ? gridOverlayContainer : gridUnderlayContainer, false);

            highlightImage.type = Image.Type.Sliced;
            highlightImage.fillCenter = true;
            highlightImage.sprite = highlightSprite;

            AssignHighlighColor(highlightImage);

            if (selectingHighlight != null)
            {
                // Set the selected highlight as the last sibling so that it will always be drawn ontop of all other highlights
                int secondLastIndex = selectingHighlight.transform.parent.childCount - 2;
                //parmentSelectingHighLight.transform.SetAsLastSibling();
            }

            return highlightImage;
        }
        private void AssignHighlighColor(Image highlight)
        {
            Color color = Color.white;

            if (highlightColors.Count > 0)
            {
                color = highlightColors[Random.Range(0, highlightColors.Count)];
            }
            else
            {
                Debug.LogError("[CharacterGrid] Highlight Colors is empty.");
            }

            highlight.color = color;
        }

        private Text CreateFloatingText(string text, Color color, Vector2 position)
        {
            GameObject floatingTextObject = new GameObject("found_word_floating_text", typeof(Shadow));
            RectTransform floatingTextRectT = floatingTextObject.AddComponent<RectTransform>();
            Text floatingText = floatingTextObject.AddComponent<Text>();

            floatingText.text = text;
            floatingText.font = letterFont;
            floatingText.fontSize = letterFontSize;
            floatingText.color = color;

            floatingTextRectT.anchoredPosition = position;
            floatingTextRectT.localScale = new Vector3(currentScale, currentScale, 1f);
            floatingTextRectT.anchorMin = new Vector2(0f, 1f);
            floatingTextRectT.anchorMax = new Vector2(0f, 1f);
            floatingTextRectT.SetParent(gridOverlayContainer, false);

            ContentSizeFitter csf = floatingTextObject.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

            return floatingText;

        }

        private int AssignKeepSign(int a, int b)
        {
            return (a / Mathf.Abs(a)) * Mathf.Abs(b);
        }

        #endregion
    }
}

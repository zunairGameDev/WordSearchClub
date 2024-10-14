﻿using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.WordSearch
{
    public class GameManager : SaveableManager<GameManager>
    {
        #region Enums

        public enum GameMode
        {
            Casual,
            Progress
        }

        public enum GameState
        {
            None,
            GeneratingBoard,
            BoardActive
        }



        #endregion

        #region Inspector Variables

        [Header("Data")]
        public List<CountryInfo> countryInfo;
        public List<WordsPerLevelShow> wordsPerLevelShow;
        [SerializeField] private string characters = null;
        [SerializeField] private List<CategoryInfo> categoryInfos = null;
        [SerializeField] private List<DifficultyInfo> difficultyInfos = null;
        [SerializeField] private List<GridInfo> levelDetails = null;
        public List<TextAsset> levelFiles;
        public List<TextAsset> dailyLevelfiles;
        public List<char> hintLetters = new List<char>();


        [Header("Values")]
        [SerializeField] private int startingCoins = 0;
        [SerializeField] private int startingKeys = 0;
        [SerializeField] private int numLevelsToAwardCoins = 0;
        [SerializeField] private int coinsToAward = 0;
        [SerializeField] private int coinCostWordHint = 0;
        [SerializeField] private int coinCostLetterHint = 0;

        [Header("Ads")]
        [SerializeField] private int numLevelsBetweenAds = 0;

        [Header("Components")]
        [SerializeField] private CharacterGrid characterGrid = null;
        public WordList wordList = null;
        [SerializeField] private DailyWordList dailyWordList = null;
        [SerializeField] private GameObject loadingIndicator = null;

        [Header("Debug / Testing")]
        [SerializeField] private bool disableLevelLocking = false;
        [SerializeField] private bool awardKeyEveryLevel = false;
        [SerializeField] private bool awardCoinsEveryLevel = false;

        #endregion

        #region Properties

        public override string SaveId { get { return "game_manager"; } }

        public List<CategoryInfo> CategoryInfos { get { return UpdateCategory(); } }
        public int CoinCostWordHint { get { return coinCostWordHint; } }
        public int CoinCostLetterHint { get { return coinCostLetterHint; } }

        public CategoryInfo ActiveCategoryInfo { get; private set; }
        public int ActiveDifficultyIndex { get; private set; }
        public int ActiveLevelIndex { get; private set; }
        public GameMode ActiveGameMode { get; private set; }
        public GameState ActiveGameState { get; private set; }
        public Board ActiveBoard { get; private set; }

        public Dictionary<string, Board> BoardsInProgress { get; private set; }
        public Dictionary<string, int> LastCompletedLevels { get; private set; }
        public Dictionary<string, JSONNode> SavedBoards { get; private set; }
        public HashSet<string> UnlockedCategories { get; private set; }

        public int Coins { get; set; }
        public int Keys { get; set; }
        public int NumLevelsTillAd { get; set; }
        public bool ToPlayNewMode;
        public bool ToPlayNewModeWithJsonData;
        public string longestWord;
        public bool toPlayAnimation;
        public bool toPlayDailyChallange;
        public GameObject dailyChallangeObject;
        public RectTransform wordFoundInWordGrid;
        public Image BackGroundImage;
        public int starterCount = 2;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();

            BoardsInProgress = new Dictionary<string, Board>();
            LastCompletedLevels = new Dictionary<string, int>();
            SavedBoards = new Dictionary<string, JSONNode>();
            UnlockedCategories = new HashSet<string>();
            characterGrid.GetComponent<GamePlayHelperButton>().HintButtonUpdate();
            characterGrid.Initialize();
            wordList.Initialize();
            InitSave();

#if BBG_MY_IAP
			BBG.MobileTools.IAPManager.Instance.OnProductPurchased += IAPProductPurchased;
#endif
        }

        #endregion

        #region Public Methods



        /// <summary>
        /// Starts the given level in the given category
        /// </summary>
        public void StartLevel(CategoryInfo categoryInfo, int levelIndex)
        {
            ActiveCategoryInfo = categoryInfo;
            ActiveDifficultyIndex = -1;
            ActiveLevelIndex = levelIndex;
            ActiveGameMode = GameMode.Progress;

            // First check if there is a saved Board we should use
            Board board = GetSavedBoard(categoryInfo, levelIndex);

            // If there is no saved board then create a Board object using the level file
            if (board == null)
            {
                board = LoadLevelFile(categoryInfo.levelFiles[levelIndex]);
            }

            SetupGame(board);

            SetBoardInProgress(board, categoryInfo, levelIndex);

            ShowGameScreen();
        }
        /// <summary>
        /// Updating CategoryInfos
        /// </summary>
        public List<CategoryInfo> UpdateCategory()
        {
            if (ToPlayNewMode)
            {
                List<CategoryInfo> choosenCategory = new List<CategoryInfo>();
                foreach (var category in categoryInfos)
                {
                    if (PlayerPrefs.GetInt("SelectedLevel") >= levelDetails.Count)
                    {
                        PlayerPrefs.DeleteKey("SelectedLevel");
                    }
                    if (category.displayName == levelDetails[PlayerPrefs.GetInt("SelectedLevel", 0)].category.ToString())
                    {
                        choosenCategory.Add(category);
                        return choosenCategory;

                    }
                }
                return null;
            }
            else { return categoryInfos; }
        }
        /// <summary>
        /// Updating Difficulty on Level Index
        /// </summary>
        public void UpdatingDifficultyInfos()
        {
            if (PlayerPrefs.GetInt("SelectedLevel") >= levelDetails.Count)
            {
                PlayerPrefs.DeleteKey("SelectedLevel");
            }

            // Get the index from PlayerPrefs, defaulting to 0 if the key doesn't exist
            int selectedLevelIndex = PlayerPrefs.GetInt("SelectedLevel", 0);

            // Retrieve the corresponding GridInfo object from levelDetails
            GridInfo selectedGridInfo = levelDetails[selectedLevelIndex];
            characterGrid.GridBackGroundSizeSetting(selectedGridInfo.gridBackGroundOffset);
            // Create a new DifficultyInfo object and map the data from GridInfo
            DifficultyInfo currentLevel = new DifficultyInfo
            {
                boardRowSize = selectedGridInfo.boardRowSize,
                boardColumnSize = selectedGridInfo.boardColumnSize,
                maxWords = selectedGridInfo.maxWords,
                maxWordLength = selectedGridInfo.maxWordLength
            };

            // Update the difficultyInfos list (assuming index 0 is where you want to store the current level)
            difficultyInfos[0] = currentLevel;
            //DifficultyInfo currentLevel = /*LoadDifficultyInfoFile(*/levelDetails[PlayerPrefs.GetInt("", 0)]/*)*/;
            //difficultyInfos[0] = currentLevel;
        }

        /// <summary>
        /// Starts the next level to be played in the given category
        /// </summary>
        public void StartNextLevel(CategoryInfo categoryInfo)
        {
            int nextLevelIndex = LastCompletedLevels.ContainsKey(categoryInfo.saveId) ? LastCompletedLevels[categoryInfo.saveId] + 1 : 0;

            if (nextLevelIndex >= categoryInfo.levelFiles.Count)
            {
                nextLevelIndex = categoryInfo.levelFiles.Count - 1;
            }

            StartLevel(categoryInfo, nextLevelIndex);
        }

        /// <summary>
        /// Starts a casual game by generating a new random board using the given category
        /// </summary>
        public void StartCasual(CategoryInfo categoryInfo, int difficultyIndex)
        {
            if (ToPlayNewMode)
            {
                if (ToPlayNewModeWithJsonData)
                {
                    if (toPlayDailyChallange)
                    {
                        dailyChallangeObject.SetActive(true);
                        ActiveDifficultyIndex = 1;
                        DailyChallange();
                        return;
                    }
                    else
                    {
                        dailyChallangeObject.SetActive(false);
                        if (PlayerPrefs.GetInt("SelectJasonLevel") >= levelFiles.Count)
                        {
                            PlayerPrefs.DeleteKey("SelectJasonLevel");
                        }

                        Board board = LoadLevelFile(levelFiles[PlayerPrefs.GetInt("SelectJasonLevel")]);


                        SetupGame(board);

                        //SetBoardInProgress(board, categoryInfo, levelIndex);

                        ShowGameScreen();
                        return;
                    }
                }
                else
                {
                    UpdatingDifficultyInfos();
                    List<CategoryInfo> categories = UpdateCategory();
                    categoryInfo = categories[0]; // Store the first element in categoryInfo
                }
            }
            ActiveCategoryInfo = categoryInfo;
            ActiveDifficultyIndex = difficultyIndex;
            ActiveLevelIndex = -1;
            ActiveGameMode = GameMode.Casual;

            // Clear the board from any previous game
            characterGrid.Clear();
            wordList.Clear();

            // Generate a new random board to use
            GenerateRandomBoard(difficultyInfos[difficultyIndex]);

            ShowGameScreen();
        }
        public void DailyChallange()
        {
            DailyBoard board = LoadDailyFile(dailyLevelfiles[0/*PlayerPrefs.GetInt("SelectJasonLevel")*/]);
            dailyWordList.Setup(board);
            GenerateDailyBoard(board);
            wordList.wordListCanvasGroup1.GetComponent<WordGenerating>().wordGridImage.sizeDelta = new Vector2(wordList.wordListCanvasGroup1.GetComponent<WordGenerating>().wordGridImage.sizeDelta.x, 308f);
            ShowGameScreen();
        }

        public void CreateNewLevelOnComplete()
        {
            //PlayerPrefs.SetInt("SelectedLevel", PlayerPrefs.GetInt("SelectedLevel") + 1);
            OnLevelCompletedPopupClosed(false, new object[] { "casual_newgame", 0 });
        }

        /// <summary>
        /// Starts a casual game using the given Board
        /// </summary>
        public void ContinueCasual(CategoryInfo categoryInfo)
        {
            Board savedBoard = GetSavedBoard(categoryInfo);

            if (savedBoard == null)
            {
                Debug.LogError("[GameManager] ContinueCasual: There is no saved casual board for category " + categoryInfo.saveId);

                return;
            }

            ActiveCategoryInfo = categoryInfo;
            ActiveDifficultyIndex = savedBoard.difficultyIndex;
            ActiveLevelIndex = -1;
            ActiveGameMode = GameMode.Casual;

            SetupGame(savedBoard);

            ShowGameScreen();
        }

        /// <summary>
        /// Sets the active category info. Used when the user selects the Levels option on the category popup to view the list of levels.
        /// </summary>
        public void SetActiveCategory(CategoryInfo categoryInfo)
        {
            ActiveCategoryInfo = categoryInfo;
        }

        /// <summary>
        /// Called when a word on the board is selected, checks if that word is a word that needs to be found.
        /// </summary>
        /// 

        public string OnWordForHintSelected(string selectedWord)
        {
            string selectedWordReversed = "";

            // Get the reverse version of the word
            for (int i = 0; i < selectedWord.Length; i++)
            {
                char character = selectedWord[i];

                selectedWordReversed = character + selectedWordReversed;
            }

            // Check if the selected word equals any of the hidden words
            for (int i = 0; i < ActiveBoard.words.Count; i++)
            {
                // Get the word and the word with no spaces without spaces
                string word = ActiveBoard.words[i];

                // Check if the word we has already been found
                if (ActiveBoard.foundWords.Contains(word))
                {
                    continue;
                }

                // Spaces are removed from the word before being places on the board so we need to compare the word without any spaces in it
                string wordNoSpaces = word.Replace(" ", "");

                // Check if the word matches the selected word or the selected word reversed
                if (selectedWord == wordNoSpaces || selectedWordReversed == wordNoSpaces)
                {
                    if (longestWord != null)
                    {
                        if (selectedWord == longestWord || selectedWordReversed == longestWord)
                        {
                            toPlayAnimation = true;
                        }
                    }
                    // Add the word to the hash set of found words for this board
                    ActiveBoard.foundWords.Add(word);

                    if (toPlayDailyChallange)
                    {
                        dailyWordList.SetWordFound(word);
                    }
                    else
                    {
                        // Notify the word list a word has been found
                        wordList.SetWordFound(word);
                    }



                    if (ActiveBoard.foundWords.Count == ActiveBoard.words.Count)
                    {
                        BoardCompleted();
                    }

                    // Return the word with the spaces
                    return word;
                }
            }

            return null;
        }

        public string OnWordSelected(string selectedWord)
        {
            string selectedWordReversed = "";
            string uppercaseSelectedWord = "";
            // Get the reverse version of the word
            for (int i = 0; i < selectedWord.Length; i++)
            {
                if (GameManager.Instance.toPlayDailyChallange)
                {
                    char character = char.ToLower(selectedWord[i]);
                    Debug.Log(character);
                    uppercaseSelectedWord = uppercaseSelectedWord + character;
                    selectedWordReversed = character + selectedWordReversed;
                }
                else
                {
                    char character = char.ToUpper(selectedWord[i]);
                    Debug.Log(character);
                    uppercaseSelectedWord = uppercaseSelectedWord + character;
                    selectedWordReversed = character + selectedWordReversed;
                }
                
            }
            selectedWord = uppercaseSelectedWord;
            // Check if the selected word equals any of the hidden words
            for (int i = 0; i < ActiveBoard.words.Count; i++)
            {
                // Get the word and the word with no spaces without spaces
                string word = ActiveBoard.words[i];

                // Check if the word we has already been found
                if (ActiveBoard.foundWords.Contains(word))
                {
                    continue;
                }

                // Spaces are removed from the word before being places on the board so we need to compare the word without any spaces in it
                string wordNoSpaces = word.Replace(" ", "");

                // Check if the word matches the selected word or the selected word reversed
                if (selectedWord == wordNoSpaces || selectedWordReversed == wordNoSpaces)
                {
                    if (longestWord != null)
                    {
                        if (selectedWord == longestWord || selectedWordReversed == longestWord)
                        {
                            toPlayAnimation = true;
                        }
                    }
                    // Add the word to the hash set of found words for this board
                    ActiveBoard.foundWords.Add(word);

                    if (toPlayDailyChallange)
                    {
                        dailyWordList.SetWordFound(word);
                    }
                    else
                    {
                        // Notify the word list a word has been found
                        wordList.SetWordFound(word);
                    }



                    if (ActiveBoard.foundWords.Count == ActiveBoard.words.Count)
                    {
                        BoardCompleted();
                    }

                    // Return the word with the spaces
                    return word;
                }
            }

            return null;
        }

        public void HintHighlightWord()
        {
            if (ActiveBoard == null)
            {
                return;
            }

            List<string> nonFoundWords = new List<string>();

            // Get all the words that have not been found yet
            for (int i = 0; i < ActiveBoard.words.Count; i++)
            {
                string word = ActiveBoard.words[i];

                if (!ActiveBoard.foundWords.Contains(word))
                {
                    nonFoundWords.Add(word);
                }
            }

            // Make sure the list is not empty
            if (nonFoundWords.Count == 0)
            {
                return;
            }

            // Check if the player has enough coins
            //if (Coins < coinCostWordHint)
            //{
            //    // Show the not enough coins popup
            //    PopupManager.Instance.Show("not_enough_coins");
            //}
            //else
            {
                // Pick a random word to show
                string wordToShow = nonFoundWords[Random.Range(0, nonFoundWords.Count)];

                // Set it as selected
                OnWordForHintSelected(wordToShow);

                // Highlight the word
                characterGrid.ShowWordHint(wordToShow);

                // Deduct the cost
                //Coins -= coinCostWordHint;

                SoundManager.Instance.Play("hint-used");
            }
        }
        public void ShowHintMultipleTimes()
        {
            for (int i = 0; i < 3; i++)
            {
                if (GlobalData.CoinCount >= 200)
                {
                    ShowingHintLetter();
                    GlobalData.CoinCount = GlobalData.CoinCount - 200;
                    MainMenuText.Instance.coinsText.text = GlobalData.CoinCount.ToString();
                }
            }
        }

        /// <summary>
        /// Shows the popup to select a letter on the board to highlight
        /// </summary>
        public void HintHighlightLetter()
        {
            if (PlayerPrefs.GetInt("StarterCounts", 2) > 0)
            {
                ShowingHintLetter();
                PlayerPrefs.SetInt("StarterCounts", PlayerPrefs.GetInt("StarterCounts", 2) - 1);

            }
            else
            if (GlobalData.CoinCount >= 100)
            {
                ShowingHintLetter();
                GlobalData.CoinCount = GlobalData.CoinCount - 100;
                MainMenuText.Instance.coinsText.text = GlobalData.CoinCount.ToString();
            }
            else
            {
                //ShowingHintLetter();
                // open Shop 
            }
            characterGrid.GetComponent<GamePlayHelperButton>().HintButtonUpdate();
        }
        public void ToShowHintLetter()
        {
            hintLetters.Clear();
            int maxLength = 0;
            // Loop through all words in ActiveBoard.words
            for (int i = 0; i < ActiveBoard.words.Count; i++)
            {
                string currentWord = ActiveBoard.words[i]; // Get the current word
                                                           // Check if the current word is the longest
                if (currentWord.Length > maxLength)
                {
                    maxLength = currentWord.Length;
                    longestWord = currentWord.Replace(" ", "");  // Update the longest word
                }
                char firstLetter = currentWord[0];
                hintLetters.Add(firstLetter);

                //// Loop through each character in the current word
                //foreach (char letter in currentWord)
                //{
                //    // If the letter is not already in hintLetters, add it
                //    if (!hintLetters.Contains(letter))
                //    {
                //        hintLetters.Add(letter); // Assuming hintLetters is a list or similar collection
                //    }
                //}
            }
        }
        public void ShowingHintLetter()
        {
            // Ensure there are letters in the hintLetters list
            if (hintLetters.Count == 0)
            {
                Debug.LogWarning("No more hint letters available!");
                return;
            }

            // Get a random index within the range of available hintLetters
            int value = Random.Range(0, hintLetters.Count);

            // Select the letter at the random index
            char letter = hintLetters[value];

            // Show the letter as a hint
            characterGrid.ShowLetterHint(letter);

            // Play the sound for using a hint
            SoundManager.Instance.Play("hint-used");

            // Remove the letter from the hintLetters to avoid repetition
            hintLetters.RemoveAt(value);
        }

        /// <summary>
        /// Returns true if the levelIndex is completed in the given category
        /// </summary>
        public bool IsLevelCompleted(CategoryInfo categoryInfo, int levelIndex)
        {
            return LastCompletedLevels.ContainsKey(categoryInfo.saveId) && levelIndex <= LastCompletedLevels[categoryInfo.saveId];
        }

        /// <summary>
        /// Returns true if the levelIndex is locked in the given category
        /// </summary>
        public bool IsLevelLocked(CategoryInfo categoryInfo, int levelIndex)
        {
            if (disableLevelLocking)
            {
                return false;
            }

            return levelIndex > 0 && (!LastCompletedLevels.ContainsKey(categoryInfo.saveId) || levelIndex > LastCompletedLevels[categoryInfo.saveId] + 1);
        }

        /// <summary>
        /// Returns true if there is a saved casual board for the given category
        /// </summary>
        public bool HasSavedCasualBoard(CategoryInfo categoryInfo)
        {
            return GetSavedBoard(categoryInfo) != null;
        }

        /// <summary>
        /// Returns true if the given category is locked
        /// </summary>
        public bool IsCategoryLocked(CategoryInfo categoryInfo)
        {
            // Check if it has been unlocked
            if (categoryInfo.lockType == CategoryInfo.LockType.None || UnlockedCategories.Contains(categoryInfo.saveId))
            {
                return false;
            }

            if (categoryInfo.lockType == CategoryInfo.LockType.IAP)
            {
#if BBG_MT_IAP
				return BBG.MobileTools.IAPManager.Instance.IsProductPurchased(categoryInfo.iapProductId);
#endif
            }

            return true;
        }

        /// <summary>
        /// Tries to unlock the category, returns true if the category was unlocked
        /// </summary>
        public bool UnlockCategory(CategoryInfo categoryInfo)
        {
            switch (categoryInfo.lockType)
            {
                case CategoryInfo.LockType.Coins:
                    if (Coins < categoryInfo.unlockAmount)
                    {
                        // Show not enough coins popup
                        PopupManager.Instance.Show("not_enough_coins");
                    }
                    else
                    {
                        // Deduct the cost of the category
                        Coins -= categoryInfo.unlockAmount;

                        UnlockedCategories.Add(categoryInfo.saveId);

                        return true;
                    }

                    break;
                case CategoryInfo.LockType.Keys:
                    if (Keys < categoryInfo.unlockAmount)
                    {
                        // Show not enough keys popup
                        PopupManager.Instance.Show("not_enough_keys");
                    }
                    else
                    {
                        // Deduct the cost of the category
                        Keys -= categoryInfo.unlockAmount;

                        UnlockedCategories.Add(categoryInfo.saveId);

                        return true;
                    }

                    break;
            }

            return false;
        }

        /// <summary>
        /// Returns true if all levels are completed in the given category
        /// </summary>
        public bool AllLevelsComplete(CategoryInfo categoryInfo)
        {
            return LastCompletedLevels.ContainsKey(categoryInfo.saveId) && LastCompletedLevels[categoryInfo.saveId] >= categoryInfo.levelFiles.Count - 1;
        }

        /// <summary>
        /// Gives the specified amount of coins
        /// </summary>
        public void GiveCoins(int amount)
        {
            Coins += amount;
        }

        #endregion

        #region Private Methods

        private void OnChooseHighlightLetterPopupClosed(bool cancelled, object[] outData)
        {
            // If the popup was not cancelled then the user selected a letter
            if (!cancelled)
            {
                // Get the letter that was selected
                char letter = (char)outData[0];

                // Set the letter as used so we can re-highlight it when loaded from save
                ActiveBoard.letterHintsUsed.Add(letter);

                // Highlight it on the board
                characterGrid.ShowLetterHint(letter);

                // Deduct the cost
                Coins -= coinCostLetterHint;

                SoundManager.Instance.Play("hint-used");
            }
        }
        public void GenerateDailyBoard(DailyBoard board)
        {
            BoardCreator.BoardConfig boardConfig = new BoardCreator.BoardConfig();
            boardConfig.rows = board.rows;
            boardConfig.cols = board.cols;
            boardConfig.words = board.words;
            boardConfig.randomCharacters = characters;
            ActiveGameState = GameState.GeneratingBoard;
            BoardCreator.CreateBoard(boardConfig, OnCasualBoardCreated);
        }

        /// <summary>
        /// Generates a random board for the current active category and difficulty
        /// </summary>
        private void GenerateRandomBoard(DifficultyInfo difficultyInfo)
        {
            // Load all the category words
            List<string> categoryWords = LoadWords(ActiveCategoryInfo, difficultyInfo.maxWordLength);

            List<string> words = new List<string>();

            // Randomly choose words to use
            for (int i = 0; i < categoryWords.Count && words.Count < difficultyInfo.maxWords; i++)
            {
                int randomIndex = Random.Range(i, categoryWords.Count);
                string randomWord = categoryWords[randomIndex];

                categoryWords[randomIndex] = categoryWords[i];
                categoryWords[i] = randomWord;

                words.Add(randomWord);
            }

            // Create the board settings that will be passed to BoardCreator.CreateBoard
            BoardCreator.BoardConfig boardConfig = new BoardCreator.BoardConfig();
            boardConfig.rows = difficultyInfo.boardRowSize;
            boardConfig.cols = difficultyInfo.boardColumnSize;
            boardConfig.words = words;
            boardConfig.randomCharacters = characters;

            ActiveGameState = GameState.GeneratingBoard;
            //loadingIndicator.SetActive(true);

            // Start the creation of the board
            BoardCreator.CreateBoard(boardConfig, OnCasualBoardCreated);
        }

        /// <summary>
        /// Invoked when BoardCreator has finished creating a randomly generated board
        /// </summary>
        private void OnCasualBoardCreated(Board board)
        {
            board.difficultyIndex = ActiveDifficultyIndex;

            SetupGame(board);

            if (!toPlayDailyChallange)
            {
                SetBoardInProgress(board, ActiveCategoryInfo);
                loadingIndicator.SetActive(false);
            }

        }

        /// <summary>
        /// Sets up the game using the given Board
        /// </summary>
        private void SetupGame(Board board)
        {
            ActiveBoard = board;

            characterGrid.Setup(board);
            if (!toPlayDailyChallange)
            {
                wordList.Setup(board);
            }

            Canvas.ForceUpdateCanvases();

            // Show all the found words
            foreach (string foundWord in board.foundWords)
            {
                characterGrid.SetWordFound(foundWord);
                wordList.SetWordFound(foundWord);
            }

            // Show all the letter hints
            foreach (char letter in board.letterHintsUsed)
            {
                characterGrid.ShowLetterHint(letter);
            }

            ActiveGameState = GameState.BoardActive;
            ToShowHintLetter();
        }

        /// <summary>
        /// Loads the list of words from the word file for the given category
        /// </summary>
        private List<string> LoadWords(CategoryInfo categoryInfo, int maxLength)
        {
            string contents = categoryInfo.wordFile.text;
            string[] lines = contents.Split('\n');

            List<string> words = new List<string>();
            HashSet<string> seenWords = new HashSet<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                string word = lines[i].TrimEnd('\r', '\n');

                if (!string.IsNullOrEmpty(word) && !seenWords.Contains(word) && word.Length <= maxLength)
                {
                    words.Add(word);
                    seenWords.Add(word);
                }
            }

            return words;
        }

        /// <summary>
        /// Invoked when all the words on the current active board have been found
        /// </summary>
        private void BoardCompleted()
        {
            if (!ToPlayNewMode)
            {
                // Remove the Board from the BoardInProgress dictionary so it can be restarted
                BoardsInProgress.Remove(GetSaveKey(ActiveCategoryInfo, ActiveLevelIndex));

                // If it was a progress level then set the last completed level index
                if (ActiveGameMode == GameMode.Progress && (!LastCompletedLevels.ContainsKey(ActiveCategoryInfo.saveId) || LastCompletedLevels[ActiveCategoryInfo.saveId] < ActiveLevelIndex))
                {
                    LastCompletedLevels[ActiveCategoryInfo.saveId] = ActiveLevelIndex;
                }

                int coinsAwarded = (numLevelsToAwardCoins == 0 || (ActiveLevelIndex + 1) % numLevelsToAwardCoins == 0 || awardCoinsEveryLevel) ? coinsToAward : 0;
                int keysAwarded = (ActiveLevelIndex == ActiveCategoryInfo.levelFiles.Count - 1 || awardKeyEveryLevel) ? 1 : 0;

                Coins += coinsAwarded;
                Keys += keysAwarded;

                // Show the level complete popup
            }
            object[] levelCompletedPopupData = { ActiveGameMode == GameMode.Progress, 10, 0, CheckingActiveLevelIndex() };

            if (ToPlayNewMode)
            {

                StartCoroutine(ShowWinPlaneAfterRotate(levelCompletedPopupData));
                //PopupManager.Instance.Show("level_completed1", levelCompletedPopupData);
            }
            else
            {
                PopupManager.Instance.Show("level_completed", levelCompletedPopupData, OnLevelCompletedPopupClosed);

            }

            SoundManager.Instance.Play("level-complete");
        }
        IEnumerator ShowWinPlaneAfterRotate(object[] levelCompletedPopupData)
        {
            characterGrid.GetComponent<ScaleAndRotate>().StartOnLevelComplete();
            yield return new WaitForSeconds(3f);
            PopupManager.Instance.Show("level_completed1", levelCompletedPopupData);
        }
        private bool CheckingActiveLevelIndex()
        {
            if (ToPlayNewMode)
            {
                return PlayerPrefs.GetInt("SelectJasonLevel") >= levelFiles.Count - 1;
            }
            else
            {
                return ActiveLevelIndex >= ActiveCategoryInfo.levelFiles.Count - 1;
            }
        }
        /// <summary>
        /// Invoked when the level completed popup has closed
        /// </summary>
        private void OnLevelCompletedPopupClosed(bool cancelled, object[] outData)
        {
            if (cancelled)
            {
                if (ScreenManager.Instance.CurrentScreenId == "game")
                {
                    ScreenManager.Instance.Back();
                }
            }
            else
            {
                switch (ActiveGameMode)
                {
                    case GameMode.Casual:
                        StartCasual(ActiveCategoryInfo, ActiveDifficultyIndex);
                        break;
                    case GameMode.Progress:
                        StartLevel(ActiveCategoryInfo, ActiveLevelIndex + 1);
                        break;
                }
            }
        }

        /// <summary>
        /// Creates a Board object from the contents of the given level file
        /// </summary>
        private Board LoadLevelFile(TextAsset levelFile)
        {
            string contents = levelFile.text;
            JSONNode json = JSON.Parse(contents);

            Board board = new Board();

            board.FromJson(json);

            return board;
        }

        private DailyBoard LoadDailyFile(TextAsset levelFile)
        {
            string contents = levelFile.text;
            JSONNode json = JSON.Parse(contents);

            DailyBoard board = new DailyBoard();

            board.FromJson(json);

            return board;
        }


        /// <summary>
        /// Update a DifficultyInfo from the contents of the given level file
        /// </summary>
        private DifficultyInfo LoadDifficultyInfoFile(TextAsset levelFile)
        {
            string contents = levelFile.text;
            JSONNode json = JSON.Parse(contents);

            DifficultyInfo difficultyInfo = new DifficultyInfo();

            difficultyInfo = JsonUtility.FromJson<DifficultyInfo>(json); /*.FromJson(json);*/

            return difficultyInfo;
        }

        /// <summary>
        /// Returns the saved board for the category and level if one exists. (levelIndex == -1 if it's a casual board)
        /// </summary>
        private Board GetSavedBoard(CategoryInfo categoryInfo, int levelIndex = -1)
        {
            string saveKey = GetSaveKey(categoryInfo, levelIndex);

            if (BoardsInProgress.ContainsKey(saveKey))
            {
                return BoardsInProgress[saveKey];
            }

            if (SavedBoards.ContainsKey(saveKey))
            {
                Board board = new Board();

                board.FromJson(SavedBoards[saveKey]);

                SavedBoards.Remove(saveKey);

                BoardsInProgress[saveKey] = board;

                return board;
            }

            return null;
        }

        /// <summary>
        /// Sets the baord in progress so it can be saved
        /// </summary>
        private void SetBoardInProgress(Board board, CategoryInfo categoryInfo, int levelIndex = -1)
        {
            string saveKey = GetSaveKey(categoryInfo, levelIndex);

            BoardsInProgress[saveKey] = board;
        }

        /// <summary>
        /// Returns the save key to use for a saved Board object
        /// </summary>
        private string GetSaveKey(CategoryInfo categoryInfo, int levelIndex = -1)
        {
            return string.Format("{0}_{1}", categoryInfo.saveId, levelIndex);
        }

        /// <summary>
        /// Shows the game screen, shows an ad if its time
        /// </summary>
        private void ShowGameScreen()
        {
            NumLevelsTillAd++;

            if (NumLevelsTillAd >= numLevelsBetweenAds)
            {
#if BBG_MT_ADS
				if (BBG.MobileTools.MobileAdsManager.Instance.ShowInterstitialAd(null))
				{
					NumLevelsTillAd = 0;
				}
#endif
            }

            ScreenManager.Instance.Show("game");
        }

        /// <summary>
        /// Invoked when a product has been purchased by the user
        /// </summary>
        private void IAPProductPurchased(string productId)
        {
            for (int i = 0; i < categoryInfos.Count; i++)
            {
                CategoryInfo categoryInfo = categoryInfos[i];

                if (categoryInfo.lockType == CategoryInfo.LockType.IAP && productId == categoryInfo.iapProductId)
                {
                    UnlockedCategories.Add(categoryInfo.saveId);
                }
            }
        }

        #endregion

        #region Save Methods

        public override Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>();
            List<object> savedBoardsData = new List<object>();
            List<object> lastCompletedLevelsData = new List<object>();

            foreach (KeyValuePair<string, Board> pair in BoardsInProgress)
            {
                Dictionary<string, object> boardData = new Dictionary<string, object>();

                boardData["key"] = pair.Key;
                boardData["board"] = pair.Value.ToJson();

                savedBoardsData.Add(boardData);
            }

            foreach (KeyValuePair<string, int> pair in LastCompletedLevels)
            {
                Dictionary<string, object> lastCompletedLevelData = new Dictionary<string, object>();

                lastCompletedLevelData["key"] = pair.Key;
                lastCompletedLevelData["index"] = pair.Value;

                lastCompletedLevelsData.Add(lastCompletedLevelData);
            }

            saveData["saved_boards"] = savedBoardsData;
            saveData["last_completed_levels"] = lastCompletedLevelsData;
            saveData["unlocked_categories"] = new List<string>(UnlockedCategories);
            saveData["coins"] = Coins;
            saveData["keys"] = Keys;
            saveData["num_levels_till_ad"] = NumLevelsTillAd;

            return saveData;
        }

        protected override void LoadSaveData(bool exists, JSONNode saveData)
        {
            if (!exists)
            {
                Coins = startingCoins;
                Keys = startingKeys;

                return;
            }

            JSONArray savedBoardsJson = saveData["saved_boards"].AsArray;
            JSONArray lastCompletedLevelsJson = saveData["last_completed_levels"].AsArray;
            JSONArray unlockedCategoriesJson = saveData["unlocked_categories"].AsArray;

            for (int i = 0; i < savedBoardsJson.Count; i++)
            {
                string key = savedBoardsJson[i]["key"].Value;
                JSONNode savedBoard = savedBoardsJson[i]["board"];

                SavedBoards[key] = savedBoard;
            }

            for (int i = 0; i < lastCompletedLevelsJson.Count; i++)
            {
                string key = lastCompletedLevelsJson[i]["key"].Value;
                int index = lastCompletedLevelsJson[i]["index"];

                LastCompletedLevels[key] = index;
            }

            for (int i = 0; i < unlockedCategoriesJson.Count; i++)
            {
                UnlockedCategories.Add(unlockedCategoriesJson[i]);
            }

            Coins = saveData["coins"].AsInt;
            Keys = saveData["keys"].AsInt;
            NumLevelsTillAd = saveData["num_levels_till_ad"].AsInt;
        }

        #endregion
    }
}

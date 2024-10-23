using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace BBG.WordSearch
{
    public class DailyWordList : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private RectTransform wordListContainer = null;
        [SerializeField] private WordListItem wordListItemPrefab = null;
        [SerializeField] private CanvasGroup wordListCanvasGroup = null;
        public bool hintWordHighlight;
        [Header("Quote UI Elements")]
        [SerializeField] private TextMeshProUGUI quoteText;  // Text for the quote with blanks

        #endregion

        #region Member Variables

        private ObjectPool wordListItemPool;
        public Dictionary<string, WordListItem> wordListItems;
        private string fullQuote;
        private List<string> missingWords;
        public List<string> words;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            wordListItemPool = new ObjectPool(wordListItemPrefab.gameObject, 10, wordListContainer);
            wordListItems = new Dictionary<string, WordListItem>();
        }

        public void Setup(DailyBoard board)
        {
            ////Clear();

            // Set the title and full quote (assuming this is passed via the Board object)
            fullQuote = board.quoteText;          // Full quote with missing words
            missingWords = board.missingWords;    // The missing words
            words = board.words;
            //hintWordHighlight = false;
            // Update the quote text with blanks
            string displayedQuote = GenerateQuoteWithBlanks(fullQuote, missingWords);
            quoteText.text = displayedQuote;

            //// Add all the words to the word list container
            //for (int i = 0; i < board.words.Count; i++)
            //{
            //    CreateWordListItem(board.words[i], wordListItemPool);
            //}

            //// Animate showing the word list
            //UIAnimation anim = UIAnimation.Alpha(wordListCanvasGroup, 0f, 1f, 0.5f);
            //anim.style = UIAnimation.Style.EaseOut;
            //anim.Play();
        }

        // Method to update the UI when a word is found
        public void SetWordFound(string word)
        {
            // Check if the word exists in the missingWords list
            if (missingWords.Contains(word))
            {
                // Remove the word from missingWords
                missingWords.Remove(word);
            }

            // Regenerate the quote with blanks for remaining missing words
            string displayedQuote = GenerateQuoteWithBlanks(fullQuote, missingWords);

            // Update the displayed quote text with the generated quote
            quoteText.text = displayedQuote;
        }

        public void Clear()
        {
            wordListItemPool.ReturnAllObjectsToPool();
            wordListItems.Clear();

            wordListContainer.sizeDelta = new Vector2(wordListContainer.sizeDelta.x, 0f);
            wordListCanvasGroup.alpha = 0f;
        }

        #endregion

        #region Private Methods

        // Generate the quote with blanks (____) for missing words
        private string GenerateQuoteWithBlanks(string quote, List<string> missingWords)
        {
            // Loop through the missing words and replace them with blanks
            for (int i = 0; i < missingWords.Count; i++)
            {
                quote = quote.Replace(missingWords[i], "____");
            }
            return quote;
        }

        // Replace a blank in the quote with the found word
        private string ReplaceBlankWithWord(string quote, string foundWord)
        {
            return quote.Replace("____", foundWord);  // Replace the first blank found with the word
        }

        // Create a word list item for the grid
        private WordListItem CreateWordListItem(string word, ObjectPool itemPool)
        {
            WordListItem wordListItem = null;

            if (!wordListItems.ContainsKey(word))
            {
                wordListItem = itemPool.GetObject<WordListItem>();
                wordListItem.Setup(word);
                wordListItems.Add(word, wordListItem);
            }
            else
            {
                Debug.LogWarning("[WordList] Board contains duplicate words. Word: " + word);
            }

            return wordListItem;
        }

        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BBG.WordSearch
{
    public class BonusList : MonoBehaviour
    {
        #region Member Variables
        [SerializeField] private Transform content;
        [SerializeField] private GameObject wordPrefab;
        public List<string> words = new List<string>();
        public List<string> foundedWords = new List<string>();
        private ObjectPool wordListItemPool;
        public Dictionary<string, WordListItem> wordListItems;

        #endregion
        public void Setup(BonusBoard board)
        {
            Clear();
            words = board.words;
            foundedWords = board.foundedWords;
            // Set the title and full quote (assuming this is passed via the Board object)


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
        public void Clear()
        {

            words.Clear();
            foundedWords.Clear();

            //wordListContainer.sizeDelta = new Vector2(wordListContainer.sizeDelta.x, 0f);
            //wordListCanvasGroup.alpha = 0f;
        }
        #region Private Methods

        private void CreateBonuListItem(string word, ObjectPool itemPool)
        {
            WordListItem wordListItem = null;

            if (!wordListItems.ContainsKey(word))
            {

            }

        }

        #endregion
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BBG.WordSearch
{
    public class WordListItem : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Text wordText = null;
        [SerializeField] private GameObject foundIndicator = null;
        [SerializeField] private Color color;

        #endregion

        #region Public Methods

        public void Setup(string word)
        {
            wordText.text = word;
            wordText.color = Color.black;
            foundIndicator.SetActive(false);
        }

        public void SetWordFound()
        {
            GameManager.Instance.GetComponent<GameManager>().wordFoundInWordGrid = this.GetComponent<RectTransform>();
            wordText.color = color;
            //foundIndicator.SetActive(true);
        }

        #endregion
    }
}

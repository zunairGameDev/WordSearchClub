using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;

namespace BBG.WordSearch
{
    public class WordListItem : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private Text wordText = null;
        [SerializeField] private GameObject foundIndicator = null;
        [SerializeField] private Color color;
        [SerializeField] private Color backgroundColor;
        public bool hintWordHighlight;

        #endregion

        #region Public Methods

        public void Setup(string word)
        {
            wordText.text = word;
            wordText.fontSize = GetFontSize(GameManager.Instance.ActiveBoard.words.Count);
            wordText.color = Color.black;
            hintWordHighlight = false;
            GetComponent<Image>().color = backgroundColor;
            foundIndicator.SetActive(false);
            AdjustRectTransformWidth();
        }

        public void SetWordFound()
        {
            GameManager.Instance.GetComponent<GameManager>().wordFoundInWordGrid = this.GetComponent<RectTransform>();
            wordText.color = color;
            GetComponent<Image>().color = backgroundColor;
            //foundIndicator.SetActive(true);
        }
        public void OnHintChangeColor(Color color)
        {

            wordText.color = Color.white;
            GetComponent<Image>().color = color;
            wordText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f).OnComplete(() => { wordText.transform.DOScale(new Vector3(1, 1, 1), 0.3f); });
        }

        private void AdjustRectTransformWidth()
        {
            // Force update the layout to get the correct preferredWidth after setting the text
            Canvas.ForceUpdateCanvases();

            // Get the preferred width based on the text content
            float preferredWidth = wordText.preferredWidth;

            // Optionally, add some padding (if needed)
            float padding = 30f; // Adjust padding according to your UI requirements

            // Set the width of the RectTransform to the preferred width plus padding
            GetComponent<RectTransform>().sizeDelta = new Vector2(preferredWidth + padding, GetComponent<RectTransform>().sizeDelta.y);
        }
        public int GetFontSize(int value)
        {
            if (value <= 8)
            {
                return 55;
            }
            else
            {
                return 46;
            }

        }
        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace BBG.WordSearch
{
    public class BonusList : MonoBehaviour
    {
        #region Member Variables
        public Transform content;
        [SerializeField] private GameObject wordPrefab;
        public Image sliderImage;
        public Image panelSliderImage;
        public List<string> words = new List<string>();
        public List<string> foundedWords = new List<string>();
        private ObjectPool wordListItemPool;
        public Dictionary<string, WordListItem> wordListItems;
        public GameObject claimButton;
        public Button closeButton;
        public Image uperHead;
        public GameObject textToShowNoWord;
        public Sprite noTextUperBar;
        public Sprite noTextFillerBar;
        public Sprite textUperBar;
        public Sprite textFillerBar;


        #endregion
        public void Setup(BonusBoard board)
        {
            Clear();
            words = board.words;
            foundedWords = board.foundedWords;
            sliderImage.fillAmount = PlayerPrefs.GetFloat("TotalHiddenWordFound") / 25f;

        }
        public void Clear()
        {

            words.Clear();
            foundedWords.Clear();

            //wordListContainer.sizeDelta = new Vector2(wordListContainer.sizeDelta.x, 0f);
            //wordListCanvasGroup.alpha = 0f;
        }
        public void IncreasingSliderValue()
        {

            PlayerPrefs.SetFloat("TotalHiddenWordFound", PlayerPrefs.GetFloat("TotalHiddenWordFound") + 1);
            sliderImage.fillAmount = PlayerPrefs.GetFloat("TotalHiddenWordFound") / 25f;
            if (PlayerPrefs.GetFloat("TotalHiddenWordFound") >= 25)
            {
                OnClickBonusButton();
                claimButton.SetActive(true);
            }
            else
            {
                claimButton.SetActive(true);
            }

        }
        public void OnClickClaimButton()
        {
            GlobalData.CoinCount += 25;
            MainMenuText.Instance.coinsText.text = GlobalData.CoinCount.ToString();
            StartCoroutine(WaitToAnimationComplete());
        }

        public IEnumerator WaitToAnimationComplete()
        {
            yield return new WaitForSeconds(0.5f);
            closeButton.onClick.Invoke();
            StartCoroutine(Unfill(1f));
        }
        IEnumerator Unfill(float duration)
        {
            float startAmount = sliderImage.fillAmount;
            float endAmount = 0;
            float elapsed = 0;
            while (elapsed < duration)
            {
                sliderImage.fillAmount = Mathf.Lerp(startAmount, endAmount, elapsed / duration);

                elapsed += Time.deltaTime;
                yield return null;
            }
            sliderImage.fillAmount = endAmount;
            panelSliderImage.fillAmount = endAmount;
            PlayerPrefs.SetFloat("TotalHiddenWordFound", endAmount);
        }
        public void OnClickBonusButton()
        {
            Debug.Log(PlayerPrefs.GetFloat("TotalHiddenWordFound") / 25);
            panelSliderImage.fillAmount = PlayerPrefs.GetFloat("TotalHiddenWordFound") / 25f;
            if (PlayerPrefs.GetFloat("TotalHiddenWordFound") >= 25)
            {
                //OnClickBonusButton();
                claimButton.SetActive(true);
            }
            else
            {
                claimButton.SetActive(false);
            }
            if (content.childCount > 0)
            {
                panelSliderImage.sprite = textFillerBar;
                uperHead.sprite = textUperBar;
                textToShowNoWord.SetActive(false);
            }
            else
            {
                panelSliderImage.sprite = noTextFillerBar;
                uperHead.sprite = noTextUperBar;
                textToShowNoWord.SetActive(true);
            }
            PopupManager.Instance.Show("BonusWord");
        }
        #region Private Methods

        public void CreateBonuListItem(string word)
        {
            GameObject hiddenWord = Instantiate(wordPrefab, content);
            hiddenWord.GetComponent<HiddenWordText>().wordName.text = word;
            hiddenWord.name = word;

        }

        #endregion
    }
}

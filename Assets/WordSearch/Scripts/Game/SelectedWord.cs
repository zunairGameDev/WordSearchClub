using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.WordSearch
{
    public class SelectedWord : MonoBehaviour
    {
        #region Inspector Variables

        [SerializeField] private TextMeshProUGUI selectedWordText = null;
        [SerializeField] private GameObject selectedWordContainer = null;
        [SerializeField] private Image selectedWordBkgImage = null;

        #endregion

        #region Public Methods

        public void SetSelectedWord(string word, Color color)
        {
            selectedWordText.text = word;
            selectedWordContainer.SetActive(true);

            selectedWordBkgImage.color = color;

        }

        public void Clear(bool shakeTheWord)
        {
            if (shakeTheWord)
            {
                ShakeWord();
            }
            else
            {
                selectedWordText.text = "";
                selectedWordContainer.SetActive(false);
            }
        }
        public void ShakeWord()
        {
            float duration = 0.7f;
            float strength = 25f;
            int shakeRepeats = 5; // Number of shake movements within the duration

            // Store the original position
            Vector3 originalPosition = transform.localPosition;

            // Create a sequence for the custom shake effect
            Sequence shakeSequence = DOTween.Sequence();

            // Calculate the duration for each shake movement
            float singleShakeDuration = duration / (shakeRepeats * 2);

            // Create a consistent shake pattern along the X-axis
            for (int i = 0; i < shakeRepeats; i++)
            {
                shakeSequence.Append(transform.DOLocalMoveX(strength, singleShakeDuration).SetRelative().SetEase(Ease.Linear))
                             .Append(transform.DOLocalMoveX(-strength, singleShakeDuration).SetRelative().SetEase(Ease.Linear));
            }

            // Return to original position at the end
            shakeSequence.Append(transform.DOLocalMove(originalPosition, singleShakeDuration).SetEase(Ease.Linear));

            // Set a callback to execute after returning to the original position
            shakeSequence.OnComplete(() =>
            {
                selectedWordText.text = "";
                selectedWordContainer.SetActive(false);
            });
        }

        #endregion
    }
}

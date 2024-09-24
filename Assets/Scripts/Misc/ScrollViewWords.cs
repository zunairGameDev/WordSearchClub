using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ScrollViewWords : MonoBehaviour
{
    #region singleton
    public static ScrollViewWords instance;
    #endregion

    private RectTransform _rect;
    public GameObject wordCellPrefab;
    public Transform scrollViewContent;

    private void Awake()
    {
        instance = this;

        _rect = GetComponent<RectTransform>();

        GameplayController gameplayController = GameplayController.Instance;

        _rect.sizeDelta = new Vector2(_rect.sizeDelta.x,/* (gameplayController.cellSize.y + gameplayController.cellSpacing.y) * gameplayController.gridSize.y*/_rect.sizeDelta.y);
    }

    public void SpawnWordCell(string word, float delay)
    {
        GameObject cell = Instantiate(wordCellPrefab, scrollViewContent);
        cell.GetComponentInChildren<TextMeshProUGUI>().text = word.ToUpper();
        cell.transform.DOScale(0, 0.3f).SetEase(Ease.OutBack).From().SetDelay(delay);
    }

    public void CheckWord(string word)
    {
        for (int i = 0; i < scrollViewContent.childCount; i++)
        {
            Text t = scrollViewContent.GetChild(i).GetComponentInChildren<Text>();

            if (t.text.ToLower() == word || t.text.ToLower() == GameplayController.Reverse(word))
            {
                HighlightBehaviour highlight = HighlightBehaviour.instance;
                int counter = (highlight.colorCounter == 0) ? (highlight.colors.Length - 1) : (highlight.colorCounter - 1);
                scrollViewContent.GetChild(i).GetComponent<Image>().color = highlight.colors[counter];
                scrollViewContent.GetChild(i).DOPunchScale(Vector3.one, 0.2f, 10, 1);
            }
        }
    }
}

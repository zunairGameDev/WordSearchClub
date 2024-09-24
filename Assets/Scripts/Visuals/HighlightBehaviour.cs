using UnityEngine;
using UnityEngine.UI.Extensions;
using DG.Tweening;
using System.Collections.Generic;

public class HighlightBehaviour : MonoBehaviour
{

    public static HighlightBehaviour instance;
    private GameObject _selectingLine;
    public float _lineThickness;

    public GameObject lineRendererPrefab;
    public Color lineColor;
    public Color[] colors;
    public int colorCounter;


    private void Awake()
    {
        instance = this;
        GameplayController.FoundWord += SetLineRenderer;
        GameplayController.SelectingWord += CreateLineRenderOnSelectingWord;
        GameplayController.ClearSelectingLine += ClearingSelectingLine;
        GameplayController.LineColorSelection += SelectingLineColor;
        GameplayController.Line_Thickness += LineRenderThickness;
        _selectingLine = Instantiate(lineRendererPrefab, transform);
    }
    private void LineRenderThickness(float thickness)
    {
        // Offset used to visualize line render behind the letter
        float offSet = 50f;
        _lineThickness = thickness + offSet;
    }
    private void SelectingLineColor()
    {
        lineColor = colors[Random.Range(0, colors.Length - 1)];
    }
    private void SetLineRenderer(RectTransform t1, RectTransform t2)
    {

        GameObject line = Instantiate(lineRendererPrefab, transform);

        line.GetComponent<UILineRenderer>().color = lineColor;
        line.GetComponent<UILineRenderer>().LineThickness = _lineThickness;
        colorCounter = (colorCounter == colors.Length - 1) ? 0 : colorCounter + 1;
        line.transform.DOScale(0, 0.3f).From().SetEase(Ease.OutBack);
        RectTransform[] points = new RectTransform[2];
        points.SetValue(t1, 0);
        points.SetValue(t2, 1);
        line.GetComponent<UILineConnector>().transforms = points;
        line.transform.SetParent(t1.parent, true);
        line.transform.SetSiblingIndex(0);
    }
    private void CreateLineRenderOnSelectingWord(List<RectTransform> tPoints)
    {
        _selectingLine.GetComponent<UILineRenderer>().color = lineColor;
        _selectingLine.GetComponent<UILineRenderer>().LineThickness = _lineThickness;
        colorCounter = (colorCounter == colors.Length - 1) ? 0 : colorCounter + 1;
        _selectingLine.transform.DOScale(1, 0.3f).From().SetEase(Ease.OutBack);
        RectTransform[] points = new RectTransform[2];
        //for (int i = 0; i < tPoints.Count; i++)
        //{
        points.SetValue(tPoints[0], 0);
        points.SetValue(tPoints[tPoints.Count - 1], 1);
        //}
        _selectingLine.GetComponent<UILineConnector>().transforms = points;
    }
    private void ClearingSelectingLine()
    {
        _selectingLine.GetComponent<UILineRenderer>().Points = new Vector2[0];
    }
    private void OnDestroy()
    {
        GameplayController.FoundWord -= SetLineRenderer;
        GameplayController.SelectingWord -= CreateLineRenderOnSelectingWord;
        GameplayController.ClearSelectingLine -= ClearingSelectingLine;
        GameplayController.LineColorSelection -= SelectingLineColor;
        GameplayController.Line_Thickness -= LineRenderThickness;
    }
}

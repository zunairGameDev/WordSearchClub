using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WordGenerating : MonoBehaviour
{
    public WordsPerLevelShow numberOfTabsToShow;
    public Transform content;
    public GameObject horizontalTabs;
    public GameObject words;
    public Transform replica;
    public RectTransform wordGridImage;

    public void GenerateHorizontalTab(WordsPerLevelShow value, Transform otherContent)
    {

        while (transform.childCount != 0)
        {
            while (transform.GetChild(0).childCount != 0)
            {
                transform.GetChild(0).GetChild(0).SetParent(otherContent);
            }
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        numberOfTabsToShow = value;
        // Adjust spacing based on the number of words in the current tab
        int horizontalTabCount = numberOfTabsToShow.numberOfWords.Count;

        wordGridImage.sizeDelta = new Vector2(wordGridImage.sizeDelta.x, WordGridImage(horizontalTabCount));

        for (int i = 0; i < numberOfTabsToShow.numberOfWords.Count; i++)
        {
            GameObject horizonatal_Tab = Instantiate(horizontalTabs, transform);

            HorizontalLayoutGroup layoutGroup = horizonatal_Tab.GetComponent<HorizontalLayoutGroup>();

            // If layout group exists, adjust the spacing based on wordsCount
            if (layoutGroup != null)
            {
                // Adjust spacing based on the number of words in the current tab
                int wordsCount = numberOfTabsToShow.numberOfWords[i].wordsCount;

                // Calculate spacing (you can tweak this formula based on design requirements)
                layoutGroup.spacing = HorizontalTab(wordsCount);
            }

            for (int j = 0; j < numberOfTabsToShow.numberOfWords[i].wordsCount; j++)
            {
                for (int k = 0; k < otherContent.childCount; k++)
                {
                    if (otherContent.GetChild(k).gameObject.activeInHierarchy)
                    {
                        otherContent.GetChild(k).SetParent(horizonatal_Tab.GetComponent<RectTransform>());
                        break;
                    }
                }

            }
        }
    }
    // Method to calculate spacing based on words count
    private float CalculateSpacing(int wordsCount)
    {
        // Adjust base values for more reasonable spacing
        float baseSpacing = -200f;    // Minimal spacing for low word count
        float maxSpacing = -500f;    // Max spacing for higher word count
        int maxWordsCount = 10;    // Max words count for scaling

        // Clamp word count to a reasonable range
        //wordsCount = Mathf.Clamp(wordsCount, 1, maxWordsCount);

        // Reverse the Lerp to reduce spacing with fewer words
        float spacing = Mathf.Lerp(baseSpacing, maxSpacing, (float)(wordsCount - 1) / (maxWordsCount - 1));

        return spacing;
    }

    public void OnClickBackButton(Transform otherContent)
    {
        //int i = 0;
        while (transform.childCount != 0)
        {
            while (transform.GetChild(0).childCount != 0)
            {
                transform.GetChild(0).GetChild(0).SetParent(otherContent);
            }
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public int WordGridImage(int value)
    {
        switch (value)
        {
            case 1: return 208;
            case 2: return 289;
            case 3: return 349;
            default: return 0;
        }
    }
    public int HorizontalTab(int value)
    {
        switch (value)
        {
            case 2: return -350;
            case 3: return -132;
            case 4: return -51;
            case 5: return 4;
            default:
                return 0;
        }
    }
}

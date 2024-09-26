using BBG.WordSearch;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDeductor : MonoBehaviour
{
    public CharacterGrid characterGrid;
    public Transform textTransform;
    public bool isEdge;
    public Vector3 originalScale;
    public Vector3 targetScale;
    public bool toIncreaseScale = true;
    public bool toDecreaseScale = true;

    private void OnEnable()
    {
        textTransform = transform.GetComponent<CharacterGridItem>().characterText.transform;
        originalScale = transform.GetComponent<CharacterGridItem>().characterText.transform.localScale;
        targetScale = originalScale * 1.2f;
    }
    public void CheckingDistance()
    {
        if (isEdge)
        {
            if (characterGrid.letterObject.Count == 1)
            {
                characterGrid.increaseDistanceAllowed = true;
            }
            else
            {
                // Check the condition
                if (characterGrid.letterObject[0].Row == characterGrid.letterObject[characterGrid.letterObject.Count - 1].Row)
                {
                    if ((characterGrid.letterObject[characterGrid.letterObject.Count - 1].Col == 0) ||
                        (characterGrid.letterObject[characterGrid.letterObject.Count - 1].Col == characterGrid.currentBoard.cols - 1))
                    {
                        characterGrid.increaseDistanceAllowed = false;
                    }
                    else
                    {
                        characterGrid.increaseDistanceAllowed = true;
                    }

                }
                else if (characterGrid.letterObject[0].Col == characterGrid.letterObject[characterGrid.letterObject.Count - 1].Col)
                {
                    if ((characterGrid.letterObject[characterGrid.letterObject.Count - 1].Row == 0) ||
                        (characterGrid.letterObject[characterGrid.letterObject.Count - 1].Row == characterGrid.currentBoard.rows - 1))
                    {
                        characterGrid.increaseDistanceAllowed = false;
                    }
                    else
                    {
                        characterGrid.increaseDistanceAllowed = true;
                    }
                }
                else
                {
                    characterGrid.increaseDistanceAllowed = false;
                }

            }
        }
        else
        {
            characterGrid.increaseDistanceAllowed = true;
        }
    }
    public void ScalingText()
    {
        if (toIncreaseScale)
        {
            // Scale up to target scale, then back to original scale (ping-pong effect)
            textTransform.DOScale(targetScale, 0.5f).SetEase(Ease.Linear);
            toIncreaseScale = false;
            toDecreaseScale = true;
        }


    }
    public void DownScalingText()
    {
        if (toDecreaseScale)
        {
            textTransform.DOScale(originalScale, 0.5f).SetEase(Ease.Linear);
            toIncreaseScale = true;
            toDecreaseScale = false;
        }
    }
}

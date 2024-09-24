using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDeductor : MonoBehaviour
{
    public CharacterGrid characterGrid;
    public bool isEdge;

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
}

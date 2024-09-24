using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New GridInfo", menuName = "GridInfo")]
public class GridInfo : ScriptableObject
{
    #region enum
    public enum Category
    {
        ANIMALS,
        ART,
        BIRDS,
        BOATS,
        BUSINESS,
        FOOD
    }
    #endregion
    public float gridBackGroundOffset;
    public Category category;
    public int boardRowSize;
    public int boardColumnSize;
    public int maxWords;
    public int maxWordLength;
}

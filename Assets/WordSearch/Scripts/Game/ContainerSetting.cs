using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerSetting : MonoBehaviour
{
    public WordListLayoutGroup wordListLayoutGroup;

    public void SettingRows(int value)
    {
        if (value <= 4)
        {
            wordListLayoutGroup.rows = 1;
        }
        else
        {
            wordListLayoutGroup.rows = 2;
        }
    }
}

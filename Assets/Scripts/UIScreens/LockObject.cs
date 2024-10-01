using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockObject : MonoBehaviour
{
    public TextMeshProUGUI labelName;
    public TextMeshProUGUI levelDespcription;

    public void ApplyingData(string name, int index)
    {
        labelName.text = name;
        levelDespcription.text = "Level " + index.ToString();
    }
}

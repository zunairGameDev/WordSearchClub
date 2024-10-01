using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class onbuttonstamps : MonoBehaviour
{
    public GameObject Stamp1;
   

    public void OnClickMe(int value)
    {
        PanelPrefabManager.instanceprefab.ActivatePanel(value);
        
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalData
{


    public static int CoinCount
    {
        get
        {
            return PlayerPrefs.GetInt("CoinsCount", 100); ;
        }
        set
        {
            PlayerPrefs.SetInt("CoinsCount", value);


        }
    }


}

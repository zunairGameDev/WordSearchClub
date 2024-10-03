using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CountryInfo
{
    public string CountryName;
    public int maxValue;

}

[System.Serializable]
public class LabelInfo
{
    public string labelName;
    public int unlockValue;
    public List<Country_Data> countryDetails;
}



[System.Serializable]
public class Country_Data
{
    public string name;
    public string title;
    public string description;
    public Sprite unlocksprit;
    public Sprite lockSprit;
    public Sprite toFindSprit;
    public int onLockSpritApply;
    public int onUnlockSpritApply;

}

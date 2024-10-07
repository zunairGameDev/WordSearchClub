using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryButtons : MonoBehaviour
{
    public Country_Data country_Data;
    public Image countryImage;

    public void ApplyDataOnButton()
    {
        if (ScrollViewController.Instance.currentIndex >= country_Data.onUnlockSpritApply)
        {
            countryImage.sprite = country_Data.unlocksprit;
        }
        else
        if (ScrollViewController.Instance.currentIndex < country_Data.onLockSpritApply)
        {

            countryImage.sprite = country_Data.toFindSprit;
        }
        else
        {
            countryImage.sprite = country_Data.lockSprit;
        }

    }

    public void OnclickThisButton()
    {
        if (ScrollViewController.Instance.currentIndex >= country_Data.onUnlockSpritApply)
        {
            ScrollViewController.Instance.StampDetails(country_Data);
        }
        else
        if (ScrollViewController.Instance.currentIndex < country_Data.onLockSpritApply)
        {

            ScrollViewController.Instance.SearchStamp();
        }
        else
        {
            int value = country_Data.onUnlockSpritApply - ScrollViewController.Instance.currentIndex;
            ScrollViewController.Instance.PlayToUnlockStamp(value);
        }

    }
}

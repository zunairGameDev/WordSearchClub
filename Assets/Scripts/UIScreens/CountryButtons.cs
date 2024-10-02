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
        if (ScrollViewController.Instance.currentIndex < country_Data.onLockSpritApply)
        {
            countryImage.sprite = country_Data.toFindSprit;
        }
        else if (ScrollViewController.Instance.currentIndex == country_Data.onLockSpritApply)
        {
            countryImage.sprite = country_Data.lockSprit;
        }
        else
        {
            countryImage.sprite = country_Data.unlocksprit;
        }
    }

    public void OnclickThisButton()
    {
        if (ScrollViewController.Instance.currentIndex < country_Data.onLockSpritApply)
        {
            ScrollViewController.Instance.SearchStamp();
        }
        else if (ScrollViewController.Instance.currentIndex == country_Data.onLockSpritApply)
        {

            ScrollViewController.Instance.PlayToUnlockStamp(country_Data.onUnlockSpritApply);
        }
        else
        {
            ScrollViewController.Instance.StampDetails(country_Data);
        }
    }
}

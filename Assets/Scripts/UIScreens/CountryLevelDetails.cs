using BBG.WordSearch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryLevelDetails : MonoBehaviour
{
    public Transform content;
    public GameObject countryPrefab;

    public void GeneratingData()
    {
        for (int i = 0; i < GameManager.Instance.countryInfo.Count; i++)
        {
            GameObject country = Instantiate(countryPrefab, content);
            country.GetComponent<CountryTab>().ApplyingData(GameManager.Instance.countryInfo[i]);
            country.name = GameManager.Instance.countryInfo[i].countryName;

        }
        this.gameObject.SetActive(true);
    }
    public void ClosingTab()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
        this.gameObject.SetActive(false);
    }
}
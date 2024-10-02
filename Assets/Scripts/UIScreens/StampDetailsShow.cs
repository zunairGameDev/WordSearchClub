using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StampDetailsShow : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI name;
    public TextMeshProUGUI description;
    public Image countryImage;

    public void ApplyingData(Country_Data country_Data)
    {
        title.text = country_Data.title;
        name.text = country_Data.name;
        description.text = country_Data.description;
        countryImage.sprite = country_Data.unlocksprit;
        this.gameObject.SetActive(true);
    }
}

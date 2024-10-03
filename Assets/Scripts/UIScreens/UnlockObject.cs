using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockObject : MonoBehaviour
{
    public List<Country_Data> countryDetails;
    public List<GameObject> countryButtons;
    public TextMeshProUGUI tabName;

    public void ApplyingDataOnButton()
    {
        for (int i = 0; i < countryDetails.Count; i++)
        {
            countryButtons[i].GetComponent<CountryButtons>().country_Data = countryDetails[i];
            countryButtons[i].GetComponent<CountryButtons>().ApplyDataOnButton();
        }
    }
}

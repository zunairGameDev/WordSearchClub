using BBG.WordSearch;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScrollViewController : MonoBehaviour
{
    public static ScrollViewController Instance;
    public List<LabelInfo> labelInfos;
    public GameObject scrollViewContent;
    public GameObject[] imagesWithTexts;
    public GameObject[] prefabs;
    public Button actionButton;
    public int currentIndex = 0;
    public Transform parentForInstantiateImages;

    public GameObject lockObject;
    public GameObject unlockObject;
    public GameObject showText;
    public GameObject stampDetailObject;

    // Stamps:
    public GameObject[] stamps_Panels;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        LockingAndUnlockingLabels();
    }
    void Start()
    {

        //actionButton.onClick.AddListener(OnButtonPress);

    }


    public void LockingAndUnlockingLabels()
    {
        currentIndex = PlayerPrefs.GetInt("SelectJasonLevel");
        for (int i = 0; i < labelInfos.Count; i++)
        {
            if (currentIndex < labelInfos[i].unlockValue)
            {
                GameObject v = Instantiate(lockObject, parentForInstantiateImages);
                v.GetComponent<LockObject>().ApplyingData(labelInfos[i].labelName, labelInfos[i].unlockValue);
            }
            else
            {
                GameObject v = Instantiate(unlockObject, parentForInstantiateImages);
                v.GetComponent<UnlockObject>().countryDetails = labelInfos[i].countryDetails;
                v.GetComponent<UnlockObject>().ApplyingDataOnButton();


            }
        }

    }
    public void SearchStamp()
    {
        showText.GetComponent<TextShowInCollection>().textdetails.text = "Search for this Stamp in the next countries!";
        showText.SetActive(true);
    }

    public void PlayToUnlockStamp(int value)
    {
        showText.GetComponent<TextShowInCollection>().textdetails.text = "Play " + value.ToString() + " more levels to collect this stamp!";
        showText.SetActive(true);
    }
    public void StampDetails(Country_Data country_Data)
    {
        stampDetailObject.GetComponent<StampDetailsShow>().ApplyingData(country_Data);
    }
}
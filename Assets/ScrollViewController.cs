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
    private int currentIndex = 0;
    public Transform parentForInstantiateImages;

    public GameObject lockObject;
    public GameObject unlockObject;

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
    void OnButtonPress()
    {
        if (currentIndex < imagesWithTexts.Length)
        {

            Destroy(imagesWithTexts[currentIndex]);

            GameObject newPrefab = Instantiate(prefabs[currentIndex], parentForInstantiateImages);
            newPrefab.transform.SetSiblingIndex(currentIndex);
            currentIndex++; // Move to the next image and prefab
        }
    }

    public void LockingAndUnlockingLabels()
    {
        currentIndex = PlayerPrefs.GetInt("SelectJasonLevel");
        for (int i = 0; i < labelInfos.Count; i++)
        {
            if(currentIndex < labelInfos[i].unlockValue)
            {
                GameObject v = Instantiate(lockObject, parentForInstantiateImages);
                v.GetComponent<LockObject>().ApplyingData(labelInfos[i].labelName, labelInfos[i].unlockValue);
            }
            else
            {

            }
        }

    }
}
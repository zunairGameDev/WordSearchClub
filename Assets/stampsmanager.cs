using UnityEngine;
using UnityEngine.UI;
public class PanelPrefabManager : MonoBehaviour
{
    public static PanelPrefabManager instanceprefab;

    public GameObject[] panelPrefabs; // Prefabs ka array jo aap ne inspector mein assign karna hai.
    public Transform parentTransform; // Jahan aap panel ko instantiate karna chahte hain (e.g., a container or canvas).
    private GameObject currentActivePanel; // Jo panel currently active hai.

    private void Awake()
    {
        instanceprefab = this;  
    }
    void Start()
    {
        
    }
    // Panel ko activate karne ka function
    public void ActivatePanel(int index)
    {
        Debug.Log("active");
        // Pehle se active panel ko destroy karen agar koi hai
        if (currentActivePanel != null)
        {
            Debug.Log("is not null");
            Destroy(currentActivePanel);
        }
        // Jo panel prefab button ke corresponding index pe hai usko instantiate karen
        if (index >= 0 && index < panelPrefabs.Length)
        {
            Debug.Log("is active");
            currentActivePanel = Instantiate(panelPrefabs[index], parentTransform);
        }
    }
}
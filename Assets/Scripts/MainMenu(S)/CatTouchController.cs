using UnityEngine;
using UnityEngine.UI;

public class CatTouchController : MonoBehaviour
{
    public GameObject panel;  // Reference to the panel that you want to show
    private Button button;    // Reference to the button component on the image

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCatImageClicked);
    }

    void OnCatImageClicked()
    {
        if (panel != null)
        {
            panel.SetActive(true); // Show the panel
        }
    }
}

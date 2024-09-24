using UnityEngine;

public class PanelController : MonoBehaviour
{
    public static PanelController Panelinstance;

    public GameObject panel; // Assign your panel in the Inspector
    public Animator CapImgAnimator; // Assign the first image's Animator
    public Animator LowerImgAnimator; // Assign the second image's Animator

    private void Awake()
    {
        Panelinstance = this;
    }

    private void Start()
    {
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().interactable = (false);
        panel.GetComponent<CanvasGroup>().blocksRaycasts = (false);
    }
    //private bool panelActivated = false;

    //void Update()
    //{
    //    // Check if the panel has been activated once
    //    if (!panelActivated)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space)) // You can change this to any condition to activate the panel
    //        {
    //            ActivatePanel();
    //            panelActivated = true;
    //        }
    //    }
    //}

    public void ActivatePanel()
    {
        //panel.SetActive(true); // Activate the panel
        panel.GetComponent<CanvasGroup>().alpha = 1;
        panel.GetComponent<CanvasGroup>().interactable = (true);
        panel.GetComponent<CanvasGroup>().blocksRaycasts = (true);
        PlayAnimations();
    }
    public void DeactivatePanel()
    {
        panel.SetActive(false); // Deactivate the panel
        PlayAnimations();
    }

    void PlayAnimations()
    {
        CapImgAnimator.Play("Cap_anim"); // Play the animation for Image 1
        LowerImgAnimator.Play("Box_anim"); // Play the animation for Image 2
    }
}

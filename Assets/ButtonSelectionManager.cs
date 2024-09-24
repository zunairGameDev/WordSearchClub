using UnityEngine;
using UnityEngine.UI;
public class ButtonSelectionManager : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    private Button selectedButton = null; // Keep track of the currently selected button
    void Start()
    {
        // Add listeners to all buttons to manage selection
        button1.onClick.AddListener(() => OnButtonClick(button1, animator1));
        button2.onClick.AddListener(() => OnButtonClick(button2, animator2));
        button3.onClick.AddListener(() => OnButtonClick(button3, animator3));
    }
    void OnButtonClick(Button button, Animator animator)
    {
        if (selectedButton != button)
        {
            // Deselect the currently selected button
            DeselectButton();
            // Select the new button
            selectedButton = button;
            animator.SetTrigger("ScaleUp"); // Trigger the scale up animation
        }
        else
        {
            // If the same button is clicked again, we keep it scaled up
            // No action needed to deselect
        }
    }
    void DeselectButton()
    {
        if (selectedButton == button1)
        {
            animator1.SetTrigger("ScaleDown"); // Trigger the scale down animation
        }
        else if (selectedButton == button2)
        {
            animator2.SetTrigger("ScaleDown");
        }
        else if (selectedButton == button3)
        {
            animator3.SetTrigger("ScaleDown");
        }
        selectedButton = null; // Clear the selected button
    }
}








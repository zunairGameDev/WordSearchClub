using UnityEngine;
using UnityEngine.UI;

public class ButtonPanelAnimator : MonoBehaviour
{
    public GameObject homePanel; // Assign in Inspector
    public GameObject challengePanel; // Assign in Inspector
    public GameObject collectionPanel; // Assign in Inspector

    public Button homeButton; // Assign in Inspector
    public Button challengeButton; // Assign in Inspector
    public Button collectionButton; // Assign in Inspector

    private Animator homeAnimator;
    private Animator challengeAnimator;
    private Animator collectionAnimator;

    void Start()
    {
        homeAnimator = homeButton.GetComponent<Animator>();
        challengeAnimator = challengeButton.GetComponent<Animator>();
        collectionAnimator = collectionButton.GetComponent<Animator>();

        // Ensure all panels are inactive at start
        homePanel.SetActive(false);
        challengePanel.SetActive(false);
        collectionPanel.SetActive(false);
    }

    public void ShowHomePanel()
    {
        // Activate home panel and deactivate others
        homePanel.SetActive(true);
        challengePanel.SetActive(false);
        collectionPanel.SetActive(false);

        // Set button animations
        homeAnimator.SetTrigger("Selected");
        challengeAnimator.ResetTrigger("Selected");
        collectionAnimator.ResetTrigger("Selected");
    }

    public void ShowChallengePanel()
    {
        // Activate challenge panel and deactivate others
        homePanel.SetActive(false);
        challengePanel.SetActive(true);
        collectionPanel.SetActive(false);

        // Set button animations
        homeAnimator.ResetTrigger("Selected");
        challengeAnimator.SetTrigger("Selected");
        collectionAnimator.ResetTrigger("Selected");
    }

    public void ShowCollectionPanel()
    {
        // Activate collection panel and deactivate others
        homePanel.SetActive(false);
        challengePanel.SetActive(false);
        collectionPanel.SetActive(true);

        // Set button animations
        homeAnimator.ResetTrigger("Selected");
        challengeAnimator.ResetTrigger("Selected");
        collectionAnimator.SetTrigger("Selected");
    }
}

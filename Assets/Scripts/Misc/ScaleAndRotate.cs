using BBG.WordSearch;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAndRotate : MonoBehaviour
{
    public Transform gridRotation;
    public Transform grid_Underlay_Container;
    public Transform grid_overlay_container;
    public Transform highligh_letter_container;
    public GameObject littleProfile;
    public GameObject wordListContainer;
    public GameObject characterGridBackGround;
    public GameObject upperBar;
    public GameObject gameSceneBackButton;
    public GameObject alignment;

    [SerializeField] private float rotationDuration = 1f; // Duration of the rotation animation
    [SerializeField] private float scaleDuration = 0.5f; // Duration of the scaling animations
    [SerializeField] private float waitDuration = 0.5f; // Time to wait between step 2 and step 3
    [SerializeField] private Vector3 initialScale = new Vector3(0.8f, 0.8f, 0.8f); // Target scale when shrinking
    [SerializeField] private Vector3 finalScale = Vector3.one; // Target scale after rotations complete 
    private Vector3 currentRotation = Vector3.zero; // To keep track of the current rotation
    private Vector3 currentChildRotation = Vector3.zero; // To keep track of the current rotation

    public Transform cellParent;
    public void GridScaleRotateAndScale()
    {
        if (GetComponent<CharacterGrid>().gridRotates)
        {
            GetComponent<CharacterGrid>().gridRotates = false;
        }
        else
        {
            GetComponent<CharacterGrid>().gridRotates = true;
        }
        //Transform gridRotation = transform;
        // Check the current rotation of the grid (parent object)
        float currentYRotation = Mathf.Round(gridRotation.localEulerAngles.z);

        // Determine the next rotation based on the current rotation state
        float rotationAngle = (currentYRotation == 0f) ? -180f : 0f;

        // Calculate the new target rotation for the parent and its children
        currentRotation = new Vector3(0, 0, rotationAngle); // Clockwise rotation for parent
        Vector3 childRotation = new Vector3(0, 0, -rotationAngle); // Anti-clockwise rotation for children

        // Create a sequence to chain animations
        Sequence sequence = DOTween.Sequence();

        // Step 1: Scale the parent down to 0.8
        sequence.Append(gridRotation.DOScale(initialScale, scaleDuration)
                            .SetEase(Ease.OutQuad)); // Optional: Set easing for the scaling
        sequence.Join(grid_Underlay_Container.DOScale(initialScale, scaleDuration)
                       .SetEase(Ease.OutQuad));
        sequence.Join(highligh_letter_container.DOScale(initialScale, scaleDuration)
                       .SetEase(Ease.OutQuad));

        // Step 2: Simultaneously rotate parent clockwise and children anti-clockwise
        sequence.AppendCallback(() =>
        {
            // Rotate parent clockwise
            gridRotation.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation
            grid_Underlay_Container.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad);
            grid_overlay_container.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad);
            highligh_letter_container.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad);
            // Rotate each child anti-clockwise
            foreach (Transform child in cellParent)
            {
                child.DOLocalRotate(childRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation
            }
        });

        // Step 2.5: Add a delay before Step 3
        sequence.AppendInterval(waitDuration); // Adding wait duration here

        // Step 3: Scale the parent back to 1 after rotation completes
        sequence.Append(gridRotation.DOScale(finalScale, scaleDuration)
                            .SetEase(Ease.OutBounce)); // Optional: Set easing for scaling back
        sequence.Join(grid_Underlay_Container.DOScale(finalScale, scaleDuration)
                        .SetEase(Ease.OutBounce));
        sequence.Join(highligh_letter_container.DOScale(finalScale, scaleDuration)
                        .SetEase(Ease.OutBounce));

        // Start the sequence
        sequence.Play();
    }

    public void ResetPanel()
    {
        GetComponent<CharacterGrid>().gridRotates = false;
        littleProfile.SetActive(true); wordListContainer.SetActive(true);
        characterGridBackGround.SetActive(true); upperBar.SetActive(true);
        gameSceneBackButton.SetActive(true); alignment.SetActive(true);
        gridRotation.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation
        grid_Underlay_Container.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad);
        grid_overlay_container.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad);
        highligh_letter_container.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad);
        foreach (Transform child in cellParent)
        {
            child.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad); // Set the easing for rotation
            child.GetChild(0).gameObject.SetActive(true);
            child.GetComponent<CharacterGridItem>().isVisible = false;
            child.GetComponent<CharacterGridItem>().hintColorAsign = false;
        }
        transform.DOLocalRotate(new Vector3(0, 0, 0), 0.1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad); // Set the easing for rotation
        transform.DOScale(new Vector3(1, 1, 1), 0.1f)
                            .SetEase(Ease.OutQuad);
    }
    public void StartOnLevelComplete()
    {
        StartCoroutine(OnLevelComplete());
    }
    IEnumerator OnLevelComplete()
    {
        yield return new WaitForSeconds(1f);
        littleProfile.SetActive(false); wordListContainer.SetActive(false);
        characterGridBackGround.SetActive(false); upperBar.SetActive(false);
        gameSceneBackButton.SetActive(false); alignment.SetActive(false);


        foreach (Transform child in cellParent)
        {
            if (!child.GetComponent<CharacterGridItem>().IsHighlighted)
            {
                child.GetChild(0).gameObject.SetActive(false);
            }

        }
        yield return new WaitForSeconds(1);
        transform.DOLocalRotate(new Vector3(0, 0, -90f), 1f, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutQuad); // Set the easing for rotation
        transform.DOScale(new Vector3(0, 0, 0f), 1f)
                            .SetEase(Ease.OutQuad);
    }
}
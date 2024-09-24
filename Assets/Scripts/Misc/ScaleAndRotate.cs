using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAndRotate : MonoBehaviour
{
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
        Transform gridRotation = transform;
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

        // Step 2: Simultaneously rotate parent clockwise and children anti-clockwise
        sequence.AppendCallback(() =>
        {
            // Rotate parent clockwise
            gridRotation.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
                     .SetEase(Ease.InOutQuad); // Set the easing for rotation

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

        // Start the sequence
        sequence.Play();

        //    Transform gridRotation = transform;
        //    // Check the current rotation of the grid (parent object)
        //    float currentYRotation = Mathf.Round(gridRotation.localEulerAngles.z);

        //    // Determine the next rotation based on the current rotation state
        //    float rotationAngle = (currentYRotation == 0f) ? -180f : 0f;

        //    // Calculate the new target rotation for the parent and its children
        //    currentRotation = new Vector3(0, 0, rotationAngle); // Update current rotation
        //    currentChildRotation = new Vector3(0, 0, -rotationAngle); // Update current rotation
        //    // Create a sequence to chain animations
        //    Sequence sequence = DOTween.Sequence();

        //    // Step 1: Scale the parent down to 0.8
        //    sequence.Append(gridRotation.DOScale(initialScale, scaleDuration)
        //                        .SetEase(Ease.OutQuad)); // Optional: Set easing for the scaling

        //    // Step 2: Simultaneously rotate parent and children
        //    sequence.AppendCallback(() =>
        //    {
        //        // Rotate parent locally
        //        gridRotation.DOLocalRotate(currentRotation, rotationDuration, RotateMode.FastBeyond360)
        //                 .SetEase(Ease.InOutQuad); // Set the easing for rotation

        //        // Rotate each child locally
        //        foreach (Transform child in cellParent)
        //        {
        //            //if (child.gameObject.CompareTag("LineRenderer"))
        //            //{
        //            //    // Skip the current child if it has the tag "LineRender"
        //            //    continue;
        //            //}
        //            child.DOLocalRotate(currentChildRotation, rotationDuration, RotateMode.FastBeyond360)
        //                 .SetEase(Ease.InOutQuad); // Set the easing for rotation
        //        }
        //    });

        //    // Step 2.5: Add a delay before Step 3
        //    sequence.AppendInterval(waitDuration);

        //    // Step 3: Scale the parent back to 1 after rotation completes
        //    sequence.Append(gridRotation.DOScale(finalScale, scaleDuration)
        //                        .SetEase(Ease.OutBounce)); // Optional: Set easing for scaling back

        //    // Start the sequence
        //    sequence.Play();
        //}
    }
}
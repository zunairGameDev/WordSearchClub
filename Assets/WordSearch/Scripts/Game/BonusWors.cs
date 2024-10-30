using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusWors : MonoBehaviour
{
    public GameObject claimButton;
    public GameObject coinButton;
    private void OnEnable()
    {
        claimButton.transform.localScale = Vector3.zero;
        if (PlayerPrefs.GetFloat("TotalHiddenWordFound") >= 25)
        {
            //OnClickBonusButton();

            MakingAnimation();
        }
        else
        {
            claimButton.SetActive(false);
        }
    }

    public void MakingAnimation()
    {
        claimButton.SetActive(true);
        Sequence scaleSequence = DOTween.Sequence();

        scaleSequence.AppendInterval(0.3f);
        // Step 1: Scale up coinButton
        scaleSequence.Append(coinButton.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.4f));

        // Add delay after Step 1
        scaleSequence.AppendInterval(0.3f); // Adjust delay as needed

        // Step 2: Scale down coinButton back to original
        scaleSequence.Append(coinButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.4f));

        // Add delay after Step 2
        scaleSequence.AppendInterval(0.3f);

        // Step 3: Scale up claimButton slightly
        scaleSequence.Append(claimButton.transform.DOScale(new Vector3(0.56f, 0.56f, 0.56f), 0.25f));

        // Add delay after Step 3
        scaleSequence.AppendInterval(0.2f);

        // Step 4: Scale down claimButton back to original
        scaleSequence.Append(claimButton.transform.DOScale(new Vector3(0.52f, 0.52f, 0.52f), 0.4f));

        // Play the sequence
        scaleSequence.Play();
    }
}
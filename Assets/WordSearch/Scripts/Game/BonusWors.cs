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

        // Step 1: Scale up coinButton
        scaleSequence.Append(coinButton.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.3f));

        // Step 2: Scale down coinButton back to original
        scaleSequence.Append(coinButton.transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f));

        // Step 3: Scale up claimButton
        scaleSequence.Append(claimButton.transform.DOScale(new Vector3(0.56f, 0.56f, 0.56f), 0.15f));
        scaleSequence.Append(claimButton.transform.DOScale(new Vector3(0.52f, 0.52f, 0.52f), 0.3f));

        // Play the sequence
        scaleSequence.Play();
    }
}
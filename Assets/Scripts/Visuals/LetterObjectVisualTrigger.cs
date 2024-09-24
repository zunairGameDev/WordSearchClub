using UnityEngine;
using DG.Tweening;

public class LetterObjectVisualTrigger : MonoBehaviour
{
    private LetterObjectScript _letterObject;

    void Awake()
    {
        _letterObject = GetComponent<LetterObjectScript>();
        _letterObject.MouseDown += MouseDown;
        _letterObject.MouseUp += MouseUp;
        _letterObject.MouseEnter += MouseEnter;
        _letterObject.MouseExit += MouseExit;

    }

    private void Start()
    {
        transform.DOScale(0, .5f).SetEase(Ease.OutBack).From();
    }

    private void OnDestroy()
    {

    }

    public void MouseDown()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.GetHighlight(), 1 + (GameplayController.Instance.highlightedObjects.Count * 0.2f));
    }

    public void MouseUp()
    {

    }

    public void MouseEnter()
    {
        if (GameplayController.Instance.activated && GameplayController.Instance.highlightedObjects.Contains(transform))
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.GetHighlight(), 1 + (GameplayController.Instance.highlightedObjects.Count * 0.2f));
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.GetSelect(), 1);
        }
        //transform.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack);
       
    }

    public void MouseExit()
    {
        //transform.DOScale(1, 0.2f).SetEase(Ease.OutBack);
    }
}

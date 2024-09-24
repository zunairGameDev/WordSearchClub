using UnityEngine;
using UnityEngine.EventSystems;

public class LetterObjectScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void ClickAction();
    public event ClickAction MouseDown;
    public event ClickAction MouseUp;
    public event ClickAction MouseExit;
    public event ClickAction MouseEnter;

    public void OnPointerDown(PointerEventData eventData)
    {
        GameplayController.Instance.LetterClick((int)Position().x, (int)Position().y, true);
        MouseDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameplayController.Instance.LetterClick((int)Position().x, (int)Position().y, false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameplayController.Instance.LetterHover((int)Position().x, (int)Position().y);
        MouseEnter();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        MouseExit();
    }
    private Vector2 Position()
    {
        string[] numbers = transform.name.Split('-');
        int x = int.Parse(numbers[0]);
        int y = int.Parse(numbers[1]);
        Vector2 position = new Vector2(x, y);
        return position;
    }

}

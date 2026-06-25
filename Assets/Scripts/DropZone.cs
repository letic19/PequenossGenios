using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string correctLetter;

    public GameManager gameManager;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped.name == correctLetter)
        {
            gameManager.CorrectAnswer();
        }
        else
        {
            gameManager.WrongAnswer();
        }
    }
}
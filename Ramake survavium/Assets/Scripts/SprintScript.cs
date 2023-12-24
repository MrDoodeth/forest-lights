using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Events;

public class SprintScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerBehaviour player;
    public UnityEvent onLongPress;
    public void OnPointerDown(PointerEventData eventData)
    {
        player.isRun = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.isRun = false;
    }
}

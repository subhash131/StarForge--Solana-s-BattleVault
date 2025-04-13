using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeInput : MonoBehaviour, IDragHandler
{
    public PlayerController playerController;

    public void OnDrag(PointerEventData eventData)
    {
        if (playerController != null)
        {
            playerController.OnSwipe(eventData.delta);
        }
    }
}

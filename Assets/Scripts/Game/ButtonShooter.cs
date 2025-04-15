using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonShooter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public PlayerController playerController;
    public float holdFireRate = 0.2f;

    private bool isHolding = false;
    private Coroutine fireCoroutine;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerController == null) return;

        // playerController.Shoot(); // Fire once on tap
        isHolding = true;
        fireCoroutine = StartCoroutine(AutoFire());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        if (fireCoroutine != null)
            StopCoroutine(fireCoroutine);
    }

    private IEnumerator AutoFire()
    {
        yield return new WaitForSeconds(holdFireRate); // Optional delay
        while (isHolding)
        {
            // playerController.Shoot();
            yield return new WaitForSeconds(holdFireRate);
        }
    }
}

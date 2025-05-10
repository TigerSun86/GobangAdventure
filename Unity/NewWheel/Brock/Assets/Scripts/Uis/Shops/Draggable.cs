using System;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isBeingDragged = false;
    private bool waitingForMouseRelease = false;

    private void Update()
    {
        if (isBeingDragged)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            transform.position = mousePosition;

            if (!waitingForMouseRelease && Input.GetMouseButtonDown(0))
            {
                waitingForMouseRelease = true;
            }

            PulseOnHover hoverTarget = WeaponUiManager.Instance.currentHoverTarget;
            if (waitingForMouseRelease && Input.GetMouseButtonUp(0) && hoverTarget != null)
            {
                isBeingDragged = false;
                waitingForMouseRelease = false;
                WeaponUiManager.Instance.currentlyDragging = null;
                SetCollidersEnabled(true);

                Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
                player.SwapWeapon(GetComponent<WeaponSuit>(), hoverTarget.gameObject);

                hoverTarget.StopPulse();
            }
        }
    }

    public void StartDragging()
    {
        isBeingDragged = true;
        waitingForMouseRelease = false;
        // WeaponUiManager.Instance.currentlyDragging = this;
        // Disable colliders to prevent interaction with PulseOnHover's
        // OnMouseEnter and OnMouseExit while dragging.
        SetCollidersEnabled(false);
    }

    private void SetCollidersEnabled(bool enabled)
    {
        GetComponent<Collider2D>().enabled = enabled;
        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = enabled;
        }
    }
}

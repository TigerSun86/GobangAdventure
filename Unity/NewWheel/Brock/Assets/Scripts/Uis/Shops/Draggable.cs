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

            if (waitingForMouseRelease && Input.GetMouseButtonUp(0))
            {
                isBeingDragged = false;
                waitingForMouseRelease = false;
                WeaponUiManager.Instance.currentlyDragging = null;

                PulseOnHover hoverTarget = WeaponUiManager.Instance.currentHoverTarget;

                if (hoverTarget != null)
                {
                    transform.parent.SetParent(hoverTarget.transform);
                    transform.parent.localPosition = Vector3.zero;
                    transform.parent.localRotation = Quaternion.identity;
                    hoverTarget.StopPulse();
                }
            }
        }
    }

    public void StartDragging()
    {
        isBeingDragged = true;
        waitingForMouseRelease = false;
        WeaponUiManager.Instance.currentlyDragging = this;
    }
}

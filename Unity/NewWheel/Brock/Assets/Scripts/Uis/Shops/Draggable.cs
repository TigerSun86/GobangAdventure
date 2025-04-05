using UnityEngine;

public class Draggable : MonoBehaviour
{
    public bool isBeingDragged = false;
    private bool waitingForMouseRelease = false;

    void Update()
    {
        if (isBeingDragged)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            transform.parent.position = mousePosition;

            if (!waitingForMouseRelease && Input.GetMouseButtonDown(0))
            {
                waitingForMouseRelease = true;
            }

            if (waitingForMouseRelease && Input.GetMouseButtonUp(0))
            {
                isBeingDragged = false;
                waitingForMouseRelease = false;
            }
        }
    }

    public void StartDragging()
    {
        isBeingDragged = true;
        waitingForMouseRelease = false;
    }
}

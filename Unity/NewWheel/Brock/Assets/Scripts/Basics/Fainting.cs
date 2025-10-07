
using UnityEngine;
using UnityEngine.Events;

public class Fainting : MonoBehaviour
{
    [SerializeField]
    public UnityEvent<GameObject> faintingEvent;

    public void CheckFaintingByHealth(int health)
    {
        if (health <= 1)
        {
            Faint();
        }
    }

    private void Faint()
    {
        faintingEvent.Invoke(gameObject);
    }
}

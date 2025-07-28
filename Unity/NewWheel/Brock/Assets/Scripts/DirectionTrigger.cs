using UnityEngine;

public class DirectionTrigger : MonoBehaviour
{
    [SerializeField, Required]
    private GameObject newTarget;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ChangeDefaultTargetPosition(newTarget.transform.position);
        }
    }
}
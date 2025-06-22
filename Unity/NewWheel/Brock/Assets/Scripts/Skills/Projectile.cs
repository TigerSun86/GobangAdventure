using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    public UnityEvent<GameObject> onHitTarget;

    [SerializeField, AssignedInCode]
    private GameObject target;

    [SerializeField, AssignedInCode]
    private float speed;

    private Rigidbody2D rb;

    public void Initialize(GameObject target, float speed)
    {
        if (speed == 0)
        {
            Debug.LogWarning("Projectile speed cannot be zero. Setting to default value of 1.");
            speed = 1f; // Default speed if not set
        }

        this.target = target;
        this.speed = speed;
    }

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (target == null || target.IsDestroyed())
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == target)
        {
            onHitTarget.Invoke(other.gameObject);
            Destroy(gameObject);
        }
    }
}
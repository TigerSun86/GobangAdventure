using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float speed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Vector3 movementVector = new Vector3();
        movementVector.x = -1;
        rb.velocity = movementVector * speed;

        if (transform.position.x < -10)
        {
            Destroy(gameObject);
        }
    }
}

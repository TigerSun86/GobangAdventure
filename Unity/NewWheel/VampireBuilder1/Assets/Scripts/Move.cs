using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    [SerializeField] FloatVariable defaultSpeed;

    [SerializeField] Vector2 direction;

    Rigidbody2D rb;

    float speed;

    public void SetSpeed(float s)
    {
        rb.velocity = direction * s;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction.Normalize();
        speed = defaultSpeed.value;
    }

    private void Start()
    {
        SetSpeed(defaultSpeed.value);
    }
}

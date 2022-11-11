using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 _inputDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        var position = (Vector2) transform.position;
        var targetPosition = position + _inputDirection;

        if (position == targetPosition) {
            return;
        }

        rb.DOMove(targetPosition, speed).SetSpeedBased();
    }
}

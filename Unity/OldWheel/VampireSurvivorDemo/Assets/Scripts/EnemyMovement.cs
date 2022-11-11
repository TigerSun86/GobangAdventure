using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private UnityEvent<Vector2> moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        var playerPosition = PlayerManager.Position;
        var position = (Vector2) transform.position;
        var direction = playerPosition - position;
        direction.Normalize();
        var targetPosition = position + direction;

        if (targetPosition == position) 
        {
            return;
        }

        rb.DOMove(targetPosition, speed).SetSpeedBased();

        moveDirection.Invoke(direction);
    }
}

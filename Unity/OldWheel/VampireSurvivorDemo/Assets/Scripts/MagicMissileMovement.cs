using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMissileMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;
    private Vector2 _direction;

    private GameObject LocateEnemy()
    {
        var results = new Collider2D[5];
        Physics2D.OverlapCircleNonAlloc(transform.position, 10, results);

        foreach (var result in results)
        {
            if (result != null && result.CompareTag("Enemy"))
            {
                return result.gameObject;
            }
        }

        return null;
    }

    private Vector2 MoveDirection(Transform target)
    {
        Vector2 direction = new Vector2(1, 0);
        if (target != null) 
        {
            direction = target.position - transform.position;
        }

        return direction;
    }

    private void Awake()
    {
        var enemy = LocateEnemy();
        var enemyTransform = enemy == null ? null : enemy.transform;
        _direction = MoveDirection(enemyTransform);
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
        transform.Rotate(new Vector3(0, 0, 90));
    }

    private void FixedUpdate()
    {
        var targetPosition = (Vector2)transform.position + _direction;
        rb.DOMove(targetPosition, speed).SetSpeedBased();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

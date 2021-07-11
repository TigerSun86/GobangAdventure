using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Vector2 currentDirection;
    private int speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        
        this.StartMove();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            StartMove();
        }
    }

    public void StartMove()
    {
        this.currentDirection = new Vector2(0, -1).normalized;
        GetComponent<Rigidbody2D>().isKinematic = false;
        GetComponent<Rigidbody2D>().velocity = this.currentDirection * speed;
        GetComponent<BallController>().transform.position = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 p = collision.GetContact(0).point;
        Debug.Log($"collision {p}");
        if (!Mathf.Approximately(Mathf.Floor(p.x), p.x) || !Mathf.Approximately(Mathf.Floor(p.y), p.y))
        {
            Debug.Log("wrong");
        }

        GameObject gameObject = collision.gameObject;
        Brick brick = gameObject.GetComponent<Brick>();
        this.currentDirection = GetNewDirection(brick.Direction);
        GetComponent<Rigidbody2D>().velocity = this.currentDirection * speed;
    }

    private Vector2 GetNewDirection(Vector2 incomingDirection) 
    {

        Vector2 result;
        int angle = Mathf.RoundToInt(Vector2.SignedAngle(currentDirection, incomingDirection));
        Debug.Log($"currentDirection {currentDirection} incomingDirection {incomingDirection} angle {angle}");
        switch (angle)
        {
            case 0:
                result = currentDirection;
                break;
            case 180:
            case -180:
            case 45:
            case -45:
                result = incomingDirection;
                break;
            case 90:
            case -90:
                result = (currentDirection + incomingDirection);
                break;
            case 135:
            case -135:
                result = Quaternion.Euler(0f, 0f, (angle * 2) / 3) * currentDirection;
                break;
            default:
                throw new NotImplementedException();

        }

        Debug.Log($"result {result}");
        return result.normalized;
    }
}

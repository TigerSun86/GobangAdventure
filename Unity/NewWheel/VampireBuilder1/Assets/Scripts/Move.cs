using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class Move : MonoBehaviour
{
    [SerializeField] FloatVariable defaultSpeed;

    [SerializeField] Vector2 direction;

    Rigidbody2D rb;

    List<SpeedChange> speedChanges = new List<SpeedChange>();

    public void ApplySpeedChange(SpeedChange speedChange)
    {
        if (speedChange.GetSpeedChangeRate() < 0)
        {
            Debug.LogError($"Could not support negative speed change rate {speedChange.GetSpeedChangeRate()}");
            return;
        }

        speedChanges.Add(speedChange);
        RefreshSpeed();
    }

    public void RemoveSpeedChange(SpeedChange speedChange)
    {
        speedChanges.Remove(speedChange);
        RefreshSpeed();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        direction.Normalize();
    }

    private void Start()
    {
        RefreshSpeed();
    }

    private void RefreshSpeed()
    {
        float rate = 1f;
        if (speedChanges.Any())
        {
            rate = speedChanges.Select(s => s.GetSpeedChangeRate()).Min();
        }

        SetSpeed(defaultSpeed.value * rate);
    }

    private void SetSpeed(float s)
    {
        rb.velocity = direction * s;
    }
}

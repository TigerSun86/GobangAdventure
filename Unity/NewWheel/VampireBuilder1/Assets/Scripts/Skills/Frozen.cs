using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Frozen : MonoBehaviour, SpeedChange
{
    [SerializeField] FloatVariable timeToLive;

    [SerializeField] UnityEvent<GameObject, float> frozenEvent;

    float timer;

    Move move;

    public float GetSpeedChangeRate()
    {
        return 0;
    }

    private void Start()
    {
        timer = timeToLive.value;

        move = this.gameObject.GetComponentInParent<Move>();
        move.ApplySpeedChange(this);
        frozenEvent.Invoke(move.gameObject, timeToLive.value);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            move.RemoveSpeedChange(this);
            Destroy(this.gameObject);
        }
    }
}
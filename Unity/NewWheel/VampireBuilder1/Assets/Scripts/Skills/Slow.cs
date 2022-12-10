using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slow : MonoBehaviour, SpeedChange
{
    [SerializeField] FloatVariable timeToLive;

    [SerializeField] FloatVariable speedChangeRate;

    [SerializeField] UnityEvent<GameObject, float> slowEvent;

    float speedChangeRateInteral;

    float timer;

    Move move;

    public float GetSpeedChangeRate()
    {
        return speedChangeRateInteral;
    }

    private void Start()
    {
        timer = timeToLive.value;
        speedChangeRateInteral = speedChangeRate.value;

        move = this.gameObject.GetComponentInParent<Move>();
        move.ApplySpeedChange(this);
        slowEvent.Invoke(move.gameObject, timeToLive.value);
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
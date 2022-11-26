using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class Healable : MonoBehaviour
{
    private Health health;

    [SerializeField] private UnityEvent<DamageData> onTakeHealing;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeHealing(int amount)
    {
        int actualAmount = health.IncreaseHealth(amount);

        onTakeHealing.Invoke(new DamageData(gameObject, amount, actualAmount, DamageType.HEALING));
    }
}

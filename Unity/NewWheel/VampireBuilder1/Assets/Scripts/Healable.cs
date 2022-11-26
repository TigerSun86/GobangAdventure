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
        health.IncreaseHealth(amount);

        onTakeHealing.Invoke(new DamageData(gameObject, amount, amount, DamageType.HEALING));
    }
}

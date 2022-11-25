using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteConfig))]
public class Damagable : MonoBehaviour
{
    private Health health;
    private SpriteConfig spriteConfig;

    [SerializeField] private UnityEvent<DamageData> onTakeDamage;

    private void Awake()
    {
        health = GetComponent<Health>();
        spriteConfig = GetComponent<SpriteConfig>();
    }

    public void TakeDamage(int attack)
    {
        health.DecreaseHealth(attack);
        spriteConfig.SetDamagedColor();
        TimersManager.SetTimer(this, 0.5f, spriteConfig.SetIdleColor);

        onTakeDamage.Invoke(new DamageData(transform.position, attack, DamageType.NORMAL_ATTACK));
    }
}

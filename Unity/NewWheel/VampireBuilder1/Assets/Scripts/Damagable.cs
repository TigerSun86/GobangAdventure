using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Timers;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(SpriteConfig))]
public class Damagable : MonoBehaviour
{
    private Health health;
    private SpriteConfig spriteConfig;

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
    }
}

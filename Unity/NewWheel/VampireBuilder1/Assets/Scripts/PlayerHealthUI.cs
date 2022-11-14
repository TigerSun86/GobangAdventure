using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Slider healthBar;

    private void Awake()
    {
        Health health = GetComponentInParent<Health>();
        healthBar = GetComponentInChildren<Slider>();

        health.healthChanged.AddListener(UpdateHealthBar);

        healthBar.maxValue = health.health;
        healthBar.value = health.health;
    }

    private void UpdateHealthBar(int health)
    {
        healthBar.value = health;
    }
}

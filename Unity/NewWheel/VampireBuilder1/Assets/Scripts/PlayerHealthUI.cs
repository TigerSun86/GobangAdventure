using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Health health;
    Slider healthBar;

    private void Awake()
    {
        health = GetComponentInParent<Health>();
        healthBar = GetComponentInChildren<Slider>();

        health.healthChanged.AddListener(UpdateHealthBar);

        healthBar.maxValue = health.health;
        healthBar.value = health.health;
    }

    private void UpdateHealthBar()
    {
        healthBar.value = health.health;
    }
}

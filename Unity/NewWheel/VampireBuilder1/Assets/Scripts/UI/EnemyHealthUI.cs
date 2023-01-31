using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    [SerializeField] Health health;

    private void Start()
    {
        health.healthChanged.AddListener(UpdateHealthUI);
        UpdateHealthUI(health.health);
    }

    private void UpdateHealthUI(int health)
    {
        text.text = health.ToString();
    }
}

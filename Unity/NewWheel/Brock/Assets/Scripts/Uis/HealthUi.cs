using UnityEngine;

public class HealthUi : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    [SerializeField] Health health;

    private void Start()
    {
        health.healthChanged.AddListener(UpdateHealthUi);
        UpdateHealthUi(health.health);
    }

    private void UpdateHealthUi(int health)
    {
        text.text = health.ToString();
    }
}

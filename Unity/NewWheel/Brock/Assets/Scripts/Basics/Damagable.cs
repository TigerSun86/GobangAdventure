using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class Damagable : MonoBehaviour
{
    private Health health;

    [SerializeField] private UnityEvent<DamageData> onTakeDamage;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    public void TakeDamage(int attack, DamageType damageType)
    {
        int actualDamage = health.DecreaseHealth(attack);
        onTakeDamage.Invoke(new DamageData(gameObject, attack, actualDamage, damageType));
    }
}

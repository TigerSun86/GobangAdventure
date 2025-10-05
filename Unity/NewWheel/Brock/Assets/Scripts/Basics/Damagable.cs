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

    public void TakeDamage(GameObject source, SkillType skillType, int attack, DamageType damageType)
    {
        int actualDamage = health.DecreaseHealth(attack);
        onTakeDamage.Invoke(new DamageData(source, skillType, gameObject, attack, actualDamage, damageType));
    }
}

using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Damagable))]
public class WeaponStand : MonoBehaviour
{
    public Health health;

    private WeaponSuit weaponSuit;

    public void Initialize(WeaponSuit weaponSuit)
    {
        this.weaponSuit = weaponSuit;
        float maxHealth = StatsManager.Instance.maxHealth + this.weaponSuit.weaponConfig.health;
        this.health.SetMaxHealth((int)maxHealth);
    }

    public WeaponSuit GetWeaponSuit()
    {
        return this.weaponSuit;
    }

    private void Awake()
    {
        this.health = GetComponent<Health>();
    }

    private void FixedUpdate()
    {
        this.transform.position = this.transform.parent.position;
    }
}

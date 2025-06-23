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
        this.health.SetMaxHealth(this.weaponSuit.weaponConfig.health);
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

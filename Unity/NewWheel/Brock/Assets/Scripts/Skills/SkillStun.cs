using Unity.VisualScripting;
using UnityEngine;

public class SkillStun : SkillBase
{
    [SerializeField, Required]
    private GameObject projectilePrefab;

    // Returns true if finished.
    protected override bool Act()
    {
        if (!AreTargetsValid())
        {
            return true;
        }

        float remainingTime = skillConfig.actionTime - timeInCurrentState;
        if (remainingTime <= 0)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity, this.transform);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            projectile.Initialize(targets[0].weaponStand.gameObject, this.skillConfig.projectileSpeed);
            projectile.onHitTarget.AddListener(OnProjectileHit);
            return true;
        }

        return false;
    }

    // Returns true if finished.
    protected override bool Recover()
    {
        float remainingTime = skillConfig.recoveryTime - timeInCurrentState;
        return remainingTime <= 0;
    }

    private void OnProjectileHit(GameObject targetObject)
    {
        if (targetObject.IsDestroyed())
        {
            return;
        }

        WeaponStand target = targetObject.GetComponent<WeaponStand>();

        double damage = this.skillConfig.value;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        Damagable damagable = target.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);

        WeaponSuit weaponSuit = target.GetWeaponSuit();
        weaponSuit.GetComponent<BuffTracker>().Add(new Buff
        {
            buffType = BuffType.Stun,
            duration = this.skillConfig.buff1.duration,
            value1 = this.skillConfig.buff1.value1
        });
    }
}
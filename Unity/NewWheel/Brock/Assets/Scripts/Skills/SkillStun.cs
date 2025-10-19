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
        Invoke(SkillEvent.SKILL_ON_PROJECTILE_HIT_UNIT, new SkillEventContext());
    }
}
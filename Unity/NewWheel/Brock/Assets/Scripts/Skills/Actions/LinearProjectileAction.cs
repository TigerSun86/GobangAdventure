using UnityEngine;

public class LinearProjectileAction : ActionBase
{
    private static readonly string PREFAB_PATH = "Prefabs/Skills/Projectile";

    private static readonly GameObject PREFAB = Resources.Load<GameObject>(PREFAB_PATH);

    private LinearProjectileActionConfig config;

    public LinearProjectileAction(LinearProjectileActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        if (LinearProjectileAction.PREFAB == null)
        {
            Debug.LogError($"Prefab not found at path: {PREFAB_PATH}");
        }

        GameObject projectileObject = UnityEngine.Object.Instantiate(LinearProjectileAction.PREFAB, this.ownerWeaponSuit.transform.position, Quaternion.identity, this.ownerWeaponSuit.transform);
        SpriteRenderer spriteRenderer = projectileObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = config.sprite;
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Initialize(target.weaponStand.gameObject, this.config.moveSpeed);
        projectile.onHitTarget.AddListener(OnProjectileHit);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }

    private void OnProjectileHit(GameObject targetObject)
    {
        this.ownerSkill.Invoke(SkillEvent.SKILL_ON_PROJECTILE_HIT_UNIT, new SkillEventContext());
    }
}

using UnityEngine;

public class Aura : MonoBehaviour
{
    private AuraModifierConfig config;

    private WeaponSuit ownerWeaponSuit;

    private SkillBase ownerSkill;

    public void Initialize(AuraModifierConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
    {
        this.config = config;
        this.ownerWeaponSuit = ownerWeaponSuit;
        this.ownerSkill = ownerSkill;
        if (!this.config.isPermanent)
        {
            Destroy(gameObject, this.config.duration);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        WeaponSuit target = collision.gameObject.GetComponent<WeaponSuit>();
        if (target != null)
        {
            if (this.config.skillTargetConfig.FilterTarget(this.ownerWeaponSuit, target, range: float.PositiveInfinity))
            {
                target.modifierContainer.AddModifier(new Modifier(this.config.childModifierConfig.Clone(), this.ownerSkill));
            }
        }
    }
}

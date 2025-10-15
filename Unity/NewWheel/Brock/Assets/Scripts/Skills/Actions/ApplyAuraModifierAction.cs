using UnityEngine;

public class ApplyAuraModifierAction : ActionBase
{
    private static readonly string AURA_PREFAB_PATH = "Prefabs/Aura";

    private ApplyAuraModifierActionConfig config;

    public ApplyAuraModifierAction(ApplyAuraModifierActionConfig config, WeaponSuit ownerWeaponSuit, SkillBase ownerSkill)
        : base(ownerWeaponSuit, ownerSkill)
    {
        this.config = config;
    }

    protected override void Apply(SkillEventContext skillEventContext, WeaponSuit target)
    {
        GameObject prefab = Resources.Load<GameObject>(AURA_PREFAB_PATH);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {AURA_PREFAB_PATH}");
        }

        AuraModifierConfig auraConfig = this.config.modifierConfig as AuraModifierConfig;
        if (auraConfig == null)
        {
            Debug.LogError("ModifierConfig is not of type AuraModifierConfig");
            return;
        }

        GameObject instance = UnityEngine.Object.Instantiate(prefab);
        instance.transform.SetParent(target.transform, false);
        Aura aura = instance.GetComponent<Aura>();
        aura.Initialize(auraConfig, this.ownerWeaponSuit, this.ownerSkill);
    }

    protected override ActionConfig GetConfig()
    {
        return this.config;
    }
}
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponSuit))]
[RequireComponent(typeof(ModifierContainer))]
public class PropertyController : MonoBehaviour
{
    private ModifierContainer modifierContainer;

    private Property baseProperty;

    private bool isDirty;

    private Property cache;

    public Property GetCurrentProperty()
    {
        if (this.isDirty)
        {
            RecalculateCache();
            this.isDirty = false;
        }

        return this.cache;
    }

    public void NotifyDirty()
    {
        this.isDirty = true;
    }

    private void Awake()
    {
        this.modifierContainer = GetComponent<ModifierContainer>();
        this.isDirty = true;
        this.baseProperty = null;
        this.cache = null;
    }

    private Property GetBaseProperty()
    {
        Property result = new Property();
        WeaponSuit weaponSuit = GetComponent<WeaponSuit>();
        result.attack = weaponSuit.weaponConfig.attackSkill.value;
        return result;
    }

    private void RecalculateCache()
    {
        if (this.baseProperty == null)
        {
            this.baseProperty = GetBaseProperty();
        }

        this.cache = this.baseProperty.Clone();
        Dictionary<ModifierPropertyType, float> modifierSums = new Dictionary<ModifierPropertyType, float>();
        foreach (Modifier modifier in this.modifierContainer.GetAllModifiers())
        {
            if (modifier.config.properties == null)
            {
                continue;
            }

            foreach (KeyValuePair<ModifierPropertyType, float> kv in modifier.config.properties)
            {
                if (!modifierSums.ContainsKey(kv.Key))
                {
                    modifierSums[kv.Key] = 0f;
                }

                modifierSums[kv.Key] += kv.Value;
            }
        }

        if (modifierSums.TryGetValue(ModifierPropertyType.ATTACK_CONSTANT, out float attackConstantSum))
        {
            this.cache.attack += attackConstantSum;
        }
    }
}
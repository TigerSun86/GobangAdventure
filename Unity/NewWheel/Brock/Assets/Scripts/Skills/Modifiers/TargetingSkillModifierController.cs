using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponSuit))]
[RequireComponent(typeof(ModifierContainer))]
public class TargetingSkillModifierController : MonoBehaviour
{
    private ModifierContainer modifierContainer;

    private bool isDirty;

    private Dictionary<(int skillIndex, ModifierStateType modifierStateType), ModifierStateValue> cache;


    public bool IsEnabled(int skillIndex, ModifierStateType modifierStateType)
    {
        if (this.isDirty)
        {
            RecalculateCache();
            this.isDirty = false;
        }

        if (this.cache.TryGetValue((skillIndex, modifierStateType), out ModifierStateValue modifierStateValue))
        {
            return modifierStateValue == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED;
        }

        return false;
    }

    public void NotifyDirty()
    {
        this.isDirty = true;
    }

    private void Awake()
    {
        this.modifierContainer = GetComponent<ModifierContainer>();
        this.isDirty = true;
        this.cache = null;
    }

    private void RecalculateCache()
    {
        this.cache = new Dictionary<(int skillIndex, ModifierStateType modifierStateType), ModifierStateValue>();
        foreach (Modifier modifier in this.modifierContainer.GetAllModifiers())
        {
            if (modifier.config.states == null || modifier.config.properties == null)
            {
                continue;
            }

            foreach (KeyValuePair<ModifierStateType, ModifierStateValue> kv in modifier.config.states)
            {
                if (kv.Key == ModifierStateType.MODIFIER_STATE_BLOCK_CD)
                {
                    if (!modifier.config.properties.TryGetValue(ModifierPropertyType.TARGETING_SKILL_INDEX, out float skillIndex))
                    {
                        Debug.LogError($"{nameof(ModifierStateType.MODIFIER_STATE_BLOCK_CD)} cannot be set without {nameof(ModifierPropertyType.TARGETING_SKILL_INDEX)}");
                        continue;
                    }

                    (int, ModifierStateType) cacheKey = ((int)skillIndex, kv.Key);
                    if (!this.cache.ContainsKey(cacheKey))
                    {
                        this.cache[cacheKey] = ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION;
                    }

                    this.cache[cacheKey] = AggregateStateValue(this.cache[cacheKey], kv.Value);
                }
            }
        }
    }


    private ModifierStateValue AggregateStateValue(ModifierStateValue value1, ModifierStateValue value2)
    {
        if (value1 == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED
            || value2 == ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED)
        {
            return ModifierStateValue.MODIFIER_STATE_VALUE_ENABLED;
        }

        if (value1 == ModifierStateValue.MODIFIER_STATE_VALUE_DISABLED
            || value2 == ModifierStateValue.MODIFIER_STATE_VALUE_DISABLED)
        {
            return ModifierStateValue.MODIFIER_STATE_VALUE_DISABLED;
        }

        return ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION;
    }
}
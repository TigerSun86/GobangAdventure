using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModifierContainer))]
public class StateController : MonoBehaviour
{
    private ModifierContainer modifierContainer;

    private bool isDirty;

    private Dictionary<ModifierStateType, ModifierStateValue> cache;

    public bool IsEnabled(ModifierStateType modifierStateType)
    {
        if (this.isDirty)
        {
            RecalculateCache();
            this.isDirty = false;
        }

        if (this.cache.TryGetValue(modifierStateType, out ModifierStateValue modifierStateValue))
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
        this.cache = new Dictionary<ModifierStateType, ModifierStateValue>();
        foreach (Modifier modifier in this.modifierContainer.GetAllModifiers())
        {
            if (modifier.config.states == null)
            {
                continue;
            }

            foreach (KeyValuePair<ModifierStateType, ModifierStateValue> kv in modifier.config.states)
            {
                if (!this.cache.ContainsKey(kv.Key))
                {
                    this.cache[kv.Key] = ModifierStateValue.MODIFIER_STATE_VALUE_NO_ACTION;
                }

                this.cache[kv.Key] = AggregateStateValue(this.cache[kv.Key], kv.Value);
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
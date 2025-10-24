using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(WeaponSuit))]
[RequireComponent(typeof(ModifierContainer))]
public class CriticalHitController : MonoBehaviour
{
    private ModifierContainer modifierContainer;

    private bool isDirty;

    private List<Property> cache;

    // Return a Property with critical_hit_multiplier, if it's a critical hit;
    // otherwise, return null.
    public Property GetCurrentProperty()
    {
        if (this.isDirty)
        {
            RecalculateCache();
            this.isDirty = false;
        }

        return this.cache.Where(p => p.critical_hit_rate > UnityEngine.Random.value).FirstOrDefault();
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
        List<Property> properties = new List<Property>();
        foreach (Modifier modifier in this.modifierContainer.GetAllModifiers())
        {
            if (modifier.config.properties == null
                || !modifier.config.properties.ContainsKey(ModifierPropertyType.CRITICAL_HIT_RATE))
            {
                continue;
            }

            if (!modifier.config.properties.ContainsKey(ModifierPropertyType.CRITICAL_HIT_MULTIPLIER))
            {
                Debug.LogError($"{nameof(ModifierPropertyType.CRITICAL_HIT_RATE)} and {nameof(ModifierPropertyType.CRITICAL_HIT_MULTIPLIER)} should always define together");
                continue;
            }

            properties.Add(new Property
            {
                critical_hit_rate = modifier.config.properties[ModifierPropertyType.CRITICAL_HIT_RATE],
                critical_hit_multiplier = modifier.config.properties[ModifierPropertyType.CRITICAL_HIT_MULTIPLIER],
            });
        }

        // Evaluate the higher multiplier first. If not a critical hit, then evaluate the next multiplier.
        this.cache = properties.OrderByDescending(p => p.critical_hit_multiplier).ToList();
    }
}
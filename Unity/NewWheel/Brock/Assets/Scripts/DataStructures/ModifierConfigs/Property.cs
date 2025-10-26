using System;
using System.Collections.Generic;

[Serializable]
public class Property
{
    public float attack;

    public float critical_hit_rate;

    public float critical_hit_multiplier;

    public Dictionary<ModifierPropertyType, float> properties = new Dictionary<ModifierPropertyType, float>();

    public virtual Property Clone()
    {
        Property clone = (Property)this.MemberwiseClone();
        if (this.properties != null)
        {
            clone.properties = new Dictionary<ModifierPropertyType, float>(this.properties);
        }

        return clone;
    }
}

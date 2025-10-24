using System;

[Serializable]
public class Property
{
    public float attack;

    public float critical_hit_rate;

    public float critical_hit_multiplier;

    public virtual Property Clone()
    {
        return (Property)this.MemberwiseClone();
    }
}
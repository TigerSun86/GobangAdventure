using System;

[Serializable]
public class Property
{
    public float attack;

    public virtual Property Clone()
    {
        return (Property)this.MemberwiseClone();
    }
}
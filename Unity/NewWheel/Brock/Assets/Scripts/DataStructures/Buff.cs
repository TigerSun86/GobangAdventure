using System;

[Serializable]
public class Buff
{
    public BuffType buffType;

    public float duration;

    public float value1;

    public float value2;

    // Set internally.
    public float startTime;

    public virtual Buff Clone()
    {
        return (Buff)this.MemberwiseClone();
    }
}
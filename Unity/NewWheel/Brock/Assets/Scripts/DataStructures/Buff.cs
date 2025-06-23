using System;

[Serializable]
public class Buff
{
    public BuffType buffType;

    public float duration;

    public float value;

    // Set internally.
    public float startTime;
}
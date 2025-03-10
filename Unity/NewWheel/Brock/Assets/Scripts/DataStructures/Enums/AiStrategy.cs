using System;

[Flags]
public enum AiStrategy
{
    None = 0,
    RunAwayWhenLowHealth = 1 << 0,
    Heal = 1 << 1,
}

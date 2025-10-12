using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
[Flags]
public enum DamageType
{
    NONE = 0,

    NORMAL_ATTACK = 1 << 0,

    WEAK_ATTACK = 1 << 1,

    STRONG_ATTACK = 1 << 2,

    HEALING = 1 << 3,

    CRITICAL_HIT = 1 << 4,
}
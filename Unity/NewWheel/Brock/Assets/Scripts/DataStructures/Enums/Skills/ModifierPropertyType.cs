using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModifierPropertyType
{
    NONE,

    ATTACK_CONSTANT,

    CRITICAL_HIT_RATE,

    CRITICAL_HIT_MULTIPLIER,
}

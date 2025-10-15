using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ActionTargetType
{
    SKILL_SELECTED,

    CASTER,

    ATTACKER,
}

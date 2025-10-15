using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModifierPropertyType
{
    NONE,

    ATTACK_CONSTANT,
}

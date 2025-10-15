using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModifierEvent
{
    MODIFIER_ON_CREATED,
}
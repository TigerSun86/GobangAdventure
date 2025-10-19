using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModifierStateType
{
    MODIFIER_STATE_STUNNED,
}

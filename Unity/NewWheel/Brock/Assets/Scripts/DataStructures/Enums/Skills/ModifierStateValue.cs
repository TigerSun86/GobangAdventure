using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum ModifierStateValue
{
    MODIFIER_STATE_VALUE_NO_ACTION,

    MODIFIER_STATE_VALUE_ENABLED,

    MODIFIER_STATE_VALUE_DISABLED,
}

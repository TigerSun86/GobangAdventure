using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum AttackActionStage
{
    STAND_BY,

    ANTICIPATION,

    RECOVERY,

    COOL_DOWN,
}
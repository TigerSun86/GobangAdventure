using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum BuffType
{
    None,

    Stun,

    CriticalHit,

    AttackAmountChange,

    LifestealPercentage,

    HealthLock,

    ReviveWhenFainting,
}

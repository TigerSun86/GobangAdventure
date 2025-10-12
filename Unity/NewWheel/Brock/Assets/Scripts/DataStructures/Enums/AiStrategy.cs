using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
[Flags]
public enum AiStrategy
{
    None = 0,
    RunAwayWhenLowHealth = 1 << 0,
    Heal = 1 << 1,
}

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
[Flags]
public enum TargetFilter
{
    None = 0,
    Self = 1 << 0,
    RunningAway = 1 << 1,
    Neighbours = 1 << 2,
    SelfAndNeighbours = Self | Neighbours,
    FullHealth = 1 << 3,
    All = Self | RunningAway | Neighbours | FullHealth,
}

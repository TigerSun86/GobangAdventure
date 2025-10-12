using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum SkillState
{
    WaitingCd,
    SelectingTarget,
    WaitingAct,
    Acting,
    Recovering,
    Completed,
}
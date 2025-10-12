using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum SkillActivationType
{
    Active,
    Passive,
    BuffBlockingCd,
}
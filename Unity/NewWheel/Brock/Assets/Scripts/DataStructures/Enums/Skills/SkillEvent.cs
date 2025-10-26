using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum SkillEvent
{
    SKILL_ON_CREATED,

    SKILL_ON_ACTING_START,

    SKILL_ON_ACTING_FINISH,

    SKILL_ON_ATTACK_LANDED,

    SKILL_ON_PROJECTILE_HIT_UNIT,

    SKILL_ON_FAINTING,
}
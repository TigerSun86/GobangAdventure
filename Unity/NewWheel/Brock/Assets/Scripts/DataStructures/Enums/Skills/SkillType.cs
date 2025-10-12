using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum SkillType
{
    Attack,
    Heal,
    Shot,
    Stun,
    CriticalHit,
    NeighbourAttackIncrease,
    NeighbourLifestealPercentage,
    Revive,
    BlinkAttack,
}

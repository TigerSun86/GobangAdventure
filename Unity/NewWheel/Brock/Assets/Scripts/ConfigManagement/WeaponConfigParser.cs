using System.Collections.Generic;
using UnityEngine;

public class WeaponConfigParser : ICsvRowParser<WeaponConfig2>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "name","level","weaponBaseType","price","health","skill1","skill2","attackValue","attackCdTime","attackActionTime","attackRecoveryTime","attackRange","spritePath"
    };

    private bool validated = false;
    private SkillConfigDb skillConfigDb;

    public WeaponConfigParser(SkillConfigDb skillConfigDb)
    {
        this.skillConfigDb = skillConfigDb;
    }

    public WeaponConfig2 ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        WeaponConfig2 result = new WeaponConfig2();
        result.attackSkill = ScriptableObject.CreateInstance<SkillConfig>();
        result.attackSkill.skillTargetConfig = new SkillTargetConfig();
        result.attackSkill.skillTargetConfig.targetType = TargetType.Opponent;
        result.attackSkill.skillTargetConfig.targetOrdering = TargetOrdering.Closest;
        result.attackSkill.skillTargetConfig.maxTargets = 1;
        result.attackSkill.skillTargetConfig.excludedTarget = TargetFilter.None;
        result.attackSkill.skillTargetConfig.includedTarget = TargetFilter.All;

        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string value = values[i].Trim();

            switch (header)
            {
                case "name":
                    result.name = value;
                    break;
                case "level":
                    result.level = ParserUtility.ParseIntSafe(value, "level");
                    break;
                case "weaponBaseType":
                    result.weaponBaseType = ParserUtility.ParseEnum<WeaponBaseType>(value, true);
                    break;
                case "price":
                    result.price = ParserUtility.ParseIntSafe(value, "price");
                    break;
                case "health":
                    result.health = ParserUtility.ParseIntSafe(value, "health");
                    break;
                case "skill1":
                    result.skill1 = string.IsNullOrWhiteSpace(value) ? null : skillConfigDb.Get(value);
                    break;
                case "skill2":
                    result.skill2 = string.IsNullOrWhiteSpace(value) ? null : skillConfigDb.Get(value);
                    break;
                case "attackValue":
                    result.attackSkill.value = ParserUtility.ParseFloatSafe(value, "attackValue");
                    break;
                case "attackCdTime":
                    result.attackSkill.cdTime = ParserUtility.ParseFloatSafe(value, "attackCdTime");
                    break;
                case "attackActionTime":
                    result.attackSkill.actionTime = ParserUtility.ParseFloatSafe(value, "attackActionTime");
                    break;
                case "attackRecoveryTime":
                    result.attackSkill.recoveryTime = ParserUtility.ParseFloatSafe(value, "attackRecoveryTime");
                    break;
                case "attackRange":
                    result.attackSkill.range = ParserUtility.ParseFloatSafe(value, "attackRange");
                    break;
                case "spritePath":
                    result.sprite = Resources.Load<Sprite>(value);
                    break;
                default:
                    Debug.LogWarning($"[WeaponConfigParser] Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

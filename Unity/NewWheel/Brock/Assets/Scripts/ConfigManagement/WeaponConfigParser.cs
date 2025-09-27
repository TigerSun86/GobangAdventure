using System.Collections.Generic;
using UnityEngine;

public class WeaponConfigParser : ICsvRowParser<WeaponConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "weaponName","level","weaponBaseType","price","health","skill1","skill2","attackValue","attackCdTime","attackActionTime","attackRecoveryTime","attackRange","experienceWorth","experienceToNextLevel","isPurchasable","spritePath"
    };

    private bool validated = false;
    private SkillConfigDb skillConfigDb;

    public WeaponConfigParser(SkillConfigDb skillConfigDb)
    {
        this.skillConfigDb = skillConfigDb;
    }

    public WeaponConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        WeaponConfig result = new WeaponConfig();
        result.attackSkill = new SkillConfig();
        result.attackSkill.skillName = "Attack";
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
                case "weaponName":
                    result.weaponName = value;
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
                    result.skill1 = string.IsNullOrWhiteSpace(value) ? new SkillConfig() : skillConfigDb.Get(value);
                    break;
                case "skill2":
                    result.skill2 = string.IsNullOrWhiteSpace(value) ? new SkillConfig() : skillConfigDb.Get(value);
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
                case "experienceWorth":
                    result.experienceWorth = ParserUtility.ParseIntSafe(value, "experienceWorth");
                    break;
                case "experienceToNextLevel":
                    result.experienceToNextLevel = ParserUtility.ParseIntSafe(value, "experienceToNextLevel");
                    break;
                case "isPurchasable":
                    result.isPurchasable = ParserUtility.ParseBoolSafe(value, "isPurchasable");
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

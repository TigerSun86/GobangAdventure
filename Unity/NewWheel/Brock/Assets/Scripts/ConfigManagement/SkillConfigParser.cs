using System.Collections.Generic;
using UnityEngine;

public class SkillConfigParser : ICsvRowParser<SkillConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "skillName","level","skillType","skillActivationType","value","cdTime","actionTime","recoveryTime","projectileSpeed","range","targetType","targetOrdering","maxTargets","excludedTarget","includedTarget","buff1Type","buff1Duration","buff1Value1","buff1Value2","description"
    };

    private bool validated = false;

    public SkillConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        SkillConfig result = new SkillConfig();
        result.skillTargetConfig = new SkillTargetConfig();
        result.buff1 = new Buff();

        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string value = values[i].Trim();

            switch (header)
            {
                case "skillName":
                    result.skillName = value;
                    break;
                case "level":
                    result.level = ParserUtility.ParseIntSafe(value, "level");
                    break;
                case "skillType":
                    result.skillType = ParserUtility.ParseEnum<SkillType>(value, true);
                    break;
                case "skillActivationType":
                    result.skillActivationType = ParserUtility.ParseEnum<SkillActivationType>(value, true);
                    break;
                case "value":
                    result.value = ParserUtility.ParseFloatSafe(value, "value");
                    break;
                case "cdTime":
                    result.cdTime = ParserUtility.ParseFloatSafe(value, "cdTime");
                    break;
                case "actionTime":
                    result.actionTime = ParserUtility.ParseFloatSafe(value, "actionTime");
                    break;
                case "recoveryTime":
                    result.recoveryTime = ParserUtility.ParseFloatSafe(value, "recoveryTime");
                    break;
                case "projectileSpeed":
                    result.projectileSpeed = ParserUtility.ParseFloatSafe(value, "projectileSpeed");
                    break;
                case "range":
                    result.range = ParserUtility.ParseFloatSafe(value, "range");
                    break;
                case "targetType":
                    result.skillTargetConfig.targetType = ParserUtility.ParseEnum<TargetType>(value, true);
                    break;
                case "targetOrdering":
                    result.skillTargetConfig.targetOrdering = ParserUtility.ParseEnum<TargetOrdering>(value, true);
                    break;
                case "maxTargets":
                    result.skillTargetConfig.maxTargets = ParserUtility.ParseIntSafe(value, "maxTargets");
                    break;
                case "excludedTarget":
                    result.skillTargetConfig.excludedTarget = ParserUtility.ParseEnum<TargetFilter>(value, true);
                    break;
                case "includedTarget":
                    result.skillTargetConfig.includedTarget = ParserUtility.ParseEnum<TargetFilter>(value, true);
                    break;
                case "buff1Type":
                    result.buff1.buffType = ParserUtility.ParseEnum<BuffType>(value, true);
                    break;
                case "buff1Duration":
                    result.buff1.duration = ParserUtility.ParseFloatSafe(value, "buff1Duration");
                    break;
                case "buff1Value1":
                    result.buff1.value1 = ParserUtility.ParseFloatSafe(value, "buff1Value1");
                    break;
                case "buff1Value2":
                    result.buff1.value2 = ParserUtility.ParseFloatSafe(value, "buff1Value2");
                    break;
                case "description":
                    result.description = value;
                    break;
                default:
                    Debug.LogWarning($"[SkillConfigParser] Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

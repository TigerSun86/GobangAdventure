using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class SubSkill : ScriptableObject
{
    public string skillName;

    public int currentLevel;

    public GemType gemType;

    public List<SubSkillLevelInfo> levelInfos;

    public MainSkill mainSkill;

    public SubSkillLevelInfo GetNextLevelInfo()
    {
        int newLevel = currentLevel + 1;
        SubSkillLevelInfo levelInfo = levelInfos.FirstOrDefault(l => l.level == newLevel);
        if (levelInfo == null)
        {
            throw new System.Exception($"Could not find the sub skill [{skillName}] for level {newLevel}");
        }

        return levelInfo;
    }

    public bool CanLevelUp()
    {
        return currentLevel < levelInfos.Max(l => l.level);
    }

    public void LevelUp()
    {
        SubSkillLevelInfo levelInfo = GetNextLevelInfo();

        ApplyChange(mainSkill, levelInfo);

        currentLevel = levelInfo.level;

        mainSkill.activeSkills.NotifyChanged();
    }

    public void SetMainSkill(MainSkill mainSkill)
    {
        this.mainSkill = mainSkill;
    }

    public void Reset()
    {
        currentLevel = 0;
    }

    private void ApplyChange(MainSkill mainSkill, SubSkillLevelInfo levelInfo)
    {
        float value = levelInfo.value;
        switch (levelInfo.attributeType)
        {
            case AttributeType.ATTACK:
                mainSkill.attack = value;
                break;
            case AttributeType.CRITICAL_RATE:
                mainSkill.criticalRate = value;
                break;
            case AttributeType.CRITICAL_AMOUNT:
                mainSkill.criticalAmount = value;
                break;
            case AttributeType.AREA:
                mainSkill.area = value;
                break;
            case AttributeType.ATTACK_DECREASE:
                mainSkill.attackDecrease = value;
                break;
            default:
                throw new InvalidEnumArgumentException(
                    nameof(levelInfo.attributeType),
                    (int)levelInfo.attributeType,
                    typeof(AttributeType));
        }
    }
}
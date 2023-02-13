using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeManager : MonoBehaviour
{
    private readonly object upgradeVariablesLock = new object();

    [SerializeField] IntVariable pendingUpgradeCount;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] TbSkillConfig tbSkillConfig;

    [SerializeField] SkillAttributeManager skillAttributeManager;

    [SerializeField] SkillId initialSkill;

    [SerializeField] GameEvent skillSelectionPendingEvent;

    [SerializeField] SkillIdGameEvent skillUpgradedEvent;

    public void LevelUp()
    {
        IncreasePendingUpgradeCountAndRaiseEvent();
    }

    public void RefreshSkillUpgradeSequence()
    {
        List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();
        foreach (SkillId skillId in skillAttributeManager.GetAllSkills())
        {
            int level = skillAttributeManager.GetLevel(skillId);
            SkillConfig skillConfig = tbSkillConfig.GetSkillConfig(skillId);
            if (level < skillConfig.GetMaxLevel())
            {
                upgradeOptions.Add(new SkillConfigUpgradeOption(skillConfig, level + 1));
            }
        }

        upgradeOptionSequence.Items.Clear();
        upgradeOptionSequence.Items.AddRange(upgradeOptions.OrderBy(s => Random.value));
    }

    public void UpgradeSkill(SkillId skillId)
    {
        int nextLevel = skillAttributeManager.GetLevel(skillId) + 1;
        SkillConfig skillConfig = tbSkillConfig.GetSkillConfig(skillId);
        if (nextLevel > skillConfig.GetMaxLevel())
        {
            Debug.LogError($"Unsupport level {nextLevel} for skill [{skillId}]");
            return;
        }

        skillAttributeManager.SetLevel(skillId, nextLevel);
        UpgradeAttributes(skillConfig, nextLevel);

        RefreshSkillUpgradeSequence();
        DecreasePendingUpgradeCountAndRaiseEvent();
        skillUpgradedEvent.Raise(skillId);
    }

    // Start is called before the first frame update
    private void Start()
    {
        skillAttributeManager.Initialize(tbSkillConfig);
        skillAttributeManager.SetLevel(initialSkill, 1);
        skillUpgradedEvent.Raise(SkillId.COMMON);

        RefreshSkillUpgradeSequence();
    }

    private void IncreasePendingUpgradeCountAndRaiseEvent()
    {
        UpdatePendingUpgradeCountAndRaiseEvent(1);
    }

    private void DecreasePendingUpgradeCountAndRaiseEvent()
    {
        UpdatePendingUpgradeCountAndRaiseEvent(-1);
    }

    private void UpdatePendingUpgradeCountAndRaiseEvent(int change)
    {
        if (change != 1 && change != -1)
        {
            Debug.LogError($"Invalid amount [{change}] to change pending upgrade count");
            return;
        }

        bool needSelection = false;
        lock (upgradeVariablesLock)
        {
            pendingUpgradeCount.ApplyChange(change);
            if ((change > 0 && pendingUpgradeCount.value == 1)
                || (change < 0 && pendingUpgradeCount.value > 0))
            {
                needSelection = true;
            }
        }

        if (needSelection)
        {
            skillSelectionPendingEvent.Raise();
        }
    }

    private void UpgradeAttributes(SkillConfig skillConfig, int level)
    {
        SkillLevelConfig levelConfig = skillConfig.GetLevelConfig(level);
        if (levelConfig.attributeType == AttributeType.NONE)
        {
            return;
        }

        SkillId skillId = SkillId.COMMON;
        if (skillConfig.skillType == SkillType.ACTIVE)
        {
            skillId = skillConfig.id;
        }

        skillAttributeManager.SetAttribute(skillId, levelConfig.attributeType, levelConfig.value);
    }
}

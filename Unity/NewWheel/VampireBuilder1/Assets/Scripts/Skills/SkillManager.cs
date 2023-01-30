using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    private readonly object upgradeVariablesLock = new object();

    [SerializeField] IntVariable pendingUpgradeCount;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] TbSkillConfig tbSkillConfig;

    [SerializeField] SkillAttributeManager skillAttributeManager;

    [SerializeField] SkillId initialSkill;

    [SerializeField] List<SkillId> enabledActiveSkills;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    [SerializeField] GameEvent skillSelectionPendingEvent;

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
        if (skillConfig.skillType == SkillType.ACTIVE && nextLevel == 1)
        {
            enabledActiveSkills.Add(skillId);
        }
        else
        {
            UpgradeAttributes(skillConfig, nextLevel);
        }

        RefreshSkillUpgradeSequence();
        DecreasePendingUpgradeCountAndRaiseEvent();
    }

    public void InstantiateSkillPrefabs(Collider2D other, GameObject bullet)
    {
        foreach (SkillId skillId in enabledActiveSkills)
        {
            if (!skillIdToPrefab.ContainsKey(skillId))
            {
                Debug.LogError($"Could not find prefab for skill [{skillId}]");
                continue;
            }

            GameObject prefab = skillIdToPrefab[skillId];
            GameObject instance = Instantiate(
                prefab,
                other.gameObject.transform.position,
                Quaternion.identity,
                this.transform);
            SkillPrefab skillPrefab = instance.GetComponent<SkillPrefab>();
            skillPrefab.target = other.gameObject;
            skillPrefab.skillAttributeManager = skillAttributeManager;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        skillAttributeManager.Initialize(tbSkillConfig);
        skillAttributeManager.SetLevel(initialSkill, 1);

        enabledActiveSkills.Add(initialSkill);

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
        SkillId skillId = SkillId.COMMON;
        if (skillConfig.skillType == SkillType.ACTIVE)
        {
            skillId = skillConfig.id;
        }

        SkillLevelConfig levelConfig = skillConfig.GetLevelConfig(level);
        skillAttributeManager.SetAttribute(skillId, levelConfig.attributeType, levelConfig.value);
    }
}

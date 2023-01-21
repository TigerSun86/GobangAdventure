using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    private readonly object upgradeVariablesLock = new object();

    [SerializeField] IntVariable pendingUpgradeCount;

    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    [SerializeField] List<Skill> skills;

    [SerializeField] MainSkill initialMainSkill;

    [SerializeField] MainSkillRuntimeSet activeSkills;

    [SerializeField] MainSkillRuntimeSet inactiveSkills;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] List<SkillNameAndPrefab> skillNameAndPrefabs;

    [SerializeField] AttributeTypeToFloatDictionary commonAttributes;

    [SerializeField] TbSkillConfig tbSkillConfig;

    [SerializeField] SkillIdToAttributesDictionary skillIdToAttributes;

    [SerializeField] SkillId initialSkill;

    [SerializeField] List<SkillId> activeSkills2;

    [SerializeField] List<SkillId> inactiveSkills2;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    [SerializeField] SkillIdToIntDictionary skillToLevelDictionary;

    [SerializeField] GameEvent skillSelectionPendingEvent;

    Dictionary<string, GameObject> skillNameAndPrefabMap;

    public void LevelUp()
    {
        IncreasePendingUpgradeCountAndRaiseEvent();
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

    public void RefreshSkillUpgradeSequence()
    {
        List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();
        foreach ((SkillId skillId, int level) in skillToLevelDictionary)
        {
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
        int nextLevel = skillToLevelDictionary[skillId] + 1;
        SkillConfig skillConfig = tbSkillConfig.GetSkillConfig(skillId);
        if (nextLevel > skillConfig.GetMaxLevel())
        {
            Debug.LogError($"Unsupport level {nextLevel} for skill [{skillId}]");
            return;
        }

        SkillLevelConfig nextLevelConfig = skillConfig.GetLevelConfig(nextLevel);
        AttributeTypeToFloatDictionary skillAttributes = skillIdToAttributes[skillId];
        skillAttributes[nextLevelConfig.attributeType] = nextLevelConfig.value;
        skillToLevelDictionary[skillId] = nextLevel;

        RefreshSkillUpgradeSequence();
        DecreasePendingUpgradeCountAndRaiseEvent();
    }

    public void InstantiateSkillPrefabs(Collider2D other, GameObject bullet)
    {
        foreach (SkillId skillId in activeSkills2)
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
            skillPrefab.commonAttributes = commonAttributes;
            skillPrefab.skillAttributes = skillIdToAttributes[SkillId.CHAIN_LIGHTNING];
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        commonAttributes = AttributeTypeToFloatDictionary.CreateInstanceWithAllAttributes();

        skillIdToAttributes.Clear();
        foreach (SkillConfig skillConfig in tbSkillConfig.GetAllSkillConfigs())
        {
            skillIdToAttributes[skillConfig.id] = skillConfig.GetInitialAttributeDictionary();
        }

        foreach (SkillId id in skillToLevelDictionary.Keys)
        {
            skillToLevelDictionary[id] = 0;
        }

        skillToLevelDictionary[initialSkill] = 1;

        activeSkills2.Clear();
        activeSkills2.Add(initialSkill);


        skillNameAndPrefabMap = new Dictionary<string, GameObject>();
        foreach (SkillNameAndPrefab pair in skillNameAndPrefabs)
        {
            skillNameAndPrefabMap[pair.skillName] = pair.prefab;
        }

        activeSkills.Clear();
        initialMainSkill.Enable();

        RefreshSkillUpgradeSequence();
    }
}

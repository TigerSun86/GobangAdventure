using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillRuntimeSet skillPrefabs;

    [SerializeField] SkillRuntimeSet skillInstances;

    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    Dictionary<Type, SkillBase> skillTypeToInstance = new Dictionary<Type, SkillBase>();

    public void RefreshSkillUpgradeSequence()
    {
        skillUpgradeSequence.Items.Clear();
        skillUpgradeSequence.Items.AddRange(
            skillInstances.Items
                .Where(s => this.isSkillUpgradable(s))
                .OrderBy(s => Random.value));
    }

    // Start is called before the first frame update
    void Start()
    {
        skillInstances.Items.Clear();
        foreach (SkillBase skillPrefab in skillPrefabs.Items)
        {
            SkillBase skillInstance = Instantiate(skillPrefab, Vector3.zero, Quaternion.identity, this.transform);
            skillInstances.Add(skillInstance);
            skillTypeToInstance[skillInstance.GetType()] = skillInstance;
        }

        RefreshSkillUpgradeSequence();
    }

    private bool isSkillUpgradable(SkillBase skill)
    {
        if (!skill.IsUpgradable())
        {
            return false;
        }

        foreach (SkillDependency dependency in skill.dependencies)
        {
            SkillBase instance = skillTypeToInstance[dependency.skill.GetType()];
            if (instance != null && instance.GetLevel() < dependency.level)
            {
                return false;
            }
        }

        return true;
    }
}

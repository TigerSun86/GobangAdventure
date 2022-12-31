using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    [SerializeField] List<Skill> skills;

    [SerializeField] MainSkill initialMainSkill;

    [SerializeField] MainSkillRuntimeSet activeSkills;

    [SerializeField] MainSkillRuntimeSet inactiveSkills;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    public void RefreshSkillUpgradeSequence()
    {
        skillUpgradeSequence.Items.Clear();
        skillUpgradeSequence.Items.AddRange(
            skills
                .Where(s => s.IsUpgradable())
                .OrderBy(s => Random.value));

        List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();

        foreach (MainSkill mainSkill in activeSkills.Items)
        {
            foreach (SubSkill subSkill in mainSkill.subSkills)
            {
                if (subSkill.CanLevelUp())
                {
                    upgradeOptions.Add(new SubSkillUpgradeOption(subSkill));
                }
            }
        }

        foreach (MainSkill mainSkill in inactiveSkills.Items)
        {
            upgradeOptions.Add(new MainSkillUpgradeOption(mainSkill));
        }

        upgradeOptionSequence.Items.Clear();
        upgradeOptionSequence.Items.AddRange(upgradeOptions.OrderBy(s => Random.value));
    }

    // Start is called before the first frame update
    void Start()
    {
        activeSkills.Clear();
        initialMainSkill.Enable();

        RefreshSkillUpgradeSequence();
    }
}

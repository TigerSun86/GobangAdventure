using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    [SerializeField] List<Skill> skills;

    [SerializeField] List<MainSkill> mainSkills;

    public void RefreshSkillUpgradeSequence()
    {
        skillUpgradeSequence.Items.Clear();
        skillUpgradeSequence.Items.AddRange(
            skills
                .Where(s => s.IsUpgradable())
                .OrderBy(s => Random.value));
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshSkillUpgradeSequence();
    }
}

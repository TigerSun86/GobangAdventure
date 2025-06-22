using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillActor : MonoBehaviour
{
    public const int PriorityHigh = 1;
    public const int PriorityNormal = 2;
    public const int PriorityLow = 3;

    [SerializeField] SkillConfig[] skillConfigs;

    public SkillBase[] skills;

    public SkillBase activeSkill;

    public PriorityQueue<SkillBase> skillActionQueue;

    private WeaponSuit weaponSuit;

    private Dictionary<SkillType, int> skillToPriorities;

    public void SetSkillPriority(SkillType skillType, int priority)
    {
        skillToPriorities[skillType] = priority;
    }

    public SkillBase GetSkillAttack()
    {
        foreach (SkillBase skill in this.skills)
        {
            if (skill is SkillAttack)
            {
                return skill;
            }
        }
        return null;
    }

    public bool IsHealing()
    {
        return this.activeSkill != null
            && this.activeSkill.skillConfig.skillType == SkillType.Heal;
    }

    public void Initialize(WeaponSuit weaponSuit)
    {
        this.weaponSuit = weaponSuit;
        this.skillConfigs = this.weaponSuit.weaponConfig.GetSkills();
        this.activeSkill = null;
        this.skillActionQueue = new PriorityQueue<SkillBase>();
        InitSkillToPriorities();

        List<SkillBase> skillList = new List<SkillBase>();
        for (int i = 0; i < this.skillConfigs.Length; i++)
        {
            SkillConfig skillConfig = this.skillConfigs[i];
            if (skillConfig == null || skillConfig.skillTargetConfig == null)
            {
                Debug.LogError($"Skill config {i} is not valid");
                continue;
            }
            SkillBase skill = null;
            switch (skillConfig.skillType)
            {
                case SkillType.Attack:
                    skill = new SkillAttack(this.weaponSuit, skillConfig);
                    break;
                case SkillType.Heal:
                    skill = new SkillHeal(this.weaponSuit, skillConfig);
                    break;
                case SkillType.Shot:
                    skill = new SkillShot(this.weaponSuit, skillConfig);
                    break;
                default:
                    Debug.LogError($"Skill type {skillConfig.skillType} not found");
                    break;
            }

            if (skill != null)
            {
                skillList.Add(skill);
            }
        }

        this.skills = skillList.ToArray();
    }

    private void FixedUpdate()
    {
        foreach (SkillBase skill in this.skills)
        {
            skill.UpdateState();
            if (skill.IsWaitingAct() && !this.skillActionQueue.Contains(skill))
            {
                this.skillActionQueue.Enqueue(skill, GetSkillPriority(skill));
            }
        }

        if (this.activeSkill != null && this.activeSkill.IsCompleted())
        {
            this.activeSkill.TriggerCd();
            if (this.activeSkill.IsWaitingAct())
            {
                this.skillActionQueue.Enqueue(this.activeSkill, GetSkillPriority(this.activeSkill));
            }

            this.activeSkill = null;
        }

        if (this.activeSkill == null)
        {
            this.activeSkill = this.skillActionQueue.DequeueOrDefault();
            // Skip the skill if it lost the target.
            while (this.activeSkill != null && !this.activeSkill.IsWaitingAct())
            {
                this.activeSkill = this.skillActionQueue.DequeueOrDefault();
            }

            if (this.activeSkill != null)
            {
                this.activeSkill.TriggerAction();
            }
        }
    }

    private void InitSkillToPriorities()
    {
        this.skillToPriorities = new Dictionary<SkillType, int>();
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
        {
            this.skillToPriorities[skillType] = PriorityNormal;
        }

        SetSkillPriority(SkillType.Heal, SkillActor.PriorityHigh);
    }

    private int GetSkillPriority(SkillBase skill)
    {
        return this.skillToPriorities[skill.skillConfig.skillType];
    }
}

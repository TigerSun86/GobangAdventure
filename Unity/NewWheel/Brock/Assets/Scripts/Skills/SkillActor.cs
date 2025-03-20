using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillActor : MonoBehaviour
{
    public const int PriorityHigh = 1;
    public const int PriorityNormal = 2;
    public const int PriorityLow = 3;

    [SerializeField]
    SkillConfig[] skillConfigs;

    public SkillBase[] skills;

    public PriorityQueue<SkillBase> skillActionQueue;

    private Dictionary<SkillType, int> skillToPriorities;

    public void SetSkillPriority(SkillType skillType, int priority)
    {
        skillToPriorities[skillType] = priority;
    }

    private void Awake()
    {
        InitSkillToPriorities();
        this.skillActionQueue = new PriorityQueue<SkillBase>();
        List<SkillBase> skillList = new List<SkillBase>();
        for (int i = 0; i < skillConfigs.Length; i++)
        {
            SkillConfig skillConfig = skillConfigs[i];
            if (skillConfig == null || skillConfig.skillTargetConfig == null)
            {
                Debug.LogError($"Skill config {i} is not valid");
                continue;
            }
            SkillBase skill = null;
            switch (skillConfig.skillType)
            {
                case SkillType.Attack:
                    skill = new SkillAttack(gameObject, skillConfig);
                    break;
                case SkillType.Heal:
                    skill = new SkillHeal(gameObject, skillConfig);
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
                this.skillActionQueue.Enqueue(skill, this.skillToPriorities[skill.skillConfig.skillType]);
            }
        }

        // Remove the completed and lost target skills from the queue.
        SkillBase firstSkill = this.skillActionQueue.Count() > 0 ? this.skillActionQueue.Peek() : null;
        while (firstSkill != null && !firstSkill.IsWaitingAct() && !firstSkill.IsActInProgress())
        {
            if (firstSkill.IsCompleted())
            {
                firstSkill.TriggerCd();
            }

            // The skill state should be Completed Or SelectingTarget.
            this.skillActionQueue.Dequeue();
            firstSkill = this.skillActionQueue.Count() > 0 ? this.skillActionQueue.Peek() : null;
        }

        if (firstSkill != null && firstSkill.IsWaitingAct())
        {
            firstSkill.TriggerAction();
        }
    }

    private void InitSkillToPriorities()
    {
        this.skillToPriorities = new Dictionary<SkillType, int>();
        foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
        {
            this.skillToPriorities[skillType] = PriorityNormal;
        }
    }
}

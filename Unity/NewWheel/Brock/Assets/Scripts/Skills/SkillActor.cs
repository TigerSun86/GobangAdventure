using System.Collections.Generic;
using UnityEngine;

public class SkillActor : MonoBehaviour
{
    [SerializeField]
    SkillConfig[] skillConfigs;

    public SkillBase[] skills;

    public Queue<SkillBase> skillActionQueue;

    private void Awake()
    {
        skillActionQueue = new Queue<SkillBase>();
        skills = new SkillBase[skillConfigs.Length];
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
                default:
                    Debug.LogError($"Skill type {skillConfig.skillType} not found");
                    break;
            }

            if (skill != null)
            {
                skills[i] = skill;
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (SkillBase skill in skills)
        {
            skill.UpdateState();
            if (skill.IsWaitingAct() && !skillActionQueue.Contains(skill))
            {
                skillActionQueue.Enqueue(skill);
            }
        }

        while (skillActionQueue.Count > 0 && skillActionQueue.Peek().IsCompleted())
        {
            skillActionQueue.Peek().TriggerCd();
            skillActionQueue.Dequeue();
        }

        if (skillActionQueue.Count > 0 && skillActionQueue.Peek().IsWaitingAct())
        {
            skillActionQueue.Peek().TriggerAction();
        }
    }
}

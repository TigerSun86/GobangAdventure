using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActor : MonoBehaviour
{
    [SerializeField]
    SkillConfig[] skillConfigs;

    public SkillBase[] skills;

    public LinkedList<SkillBase> skillActionQueue;

    private void Awake()
    {
        this.skillActionQueue = new LinkedList<SkillBase>();
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
                this.skillActionQueue.AddLast(skill);
            }
        }

        SkillBase firstSkill = this.skillActionQueue.FirstOrDefault();
        while (firstSkill != null && !firstSkill.IsWaitingAct() && !firstSkill.IsActInProgress())
        {
            if (firstSkill.IsCompleted())
            {
                firstSkill.TriggerCd();
            }

            this.skillActionQueue.RemoveFirst();
            firstSkill = this.skillActionQueue.FirstOrDefault();
        }

        if (firstSkill != null && firstSkill.IsWaitingAct())
        {
            firstSkill.TriggerAction();
        }
    }
}

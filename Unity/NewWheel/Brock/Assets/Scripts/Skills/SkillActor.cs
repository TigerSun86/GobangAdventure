using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillActor : MonoBehaviour
{
    public const int PriorityHigh = 1;
    public const int PriorityNormal = 2;
    public const int PriorityLow = 3;

    [SerializeField] SkillConfig[] skillConfigs;

    [SerializeField, AssignedInCode]
    private SkillPrefabDb skillPrefabDb;

    public SkillBase[] skills;

    public SkillBase activeSkill;

    public PriorityQueue<SkillBase> skillActionQueue;

    private WeaponSuit weaponSuit;

    private Dictionary<SkillType, int> skillToPriorities;

    private bool isInitialized = false;

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
        this.skillPrefabDb = SkillPrefabDb.Instance;
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

            GameObject skillPrefab = this.skillPrefabDb.GetSkillPrefab(skillConfig.skillType);
            if (skillPrefab == null)
            {
                Debug.LogError($"Skill prefab for {skillConfig.skillType} is not found");
                continue;
            }
            GameObject skillObject = Instantiate(skillPrefab, this.transform.position, Quaternion.identity, this.transform);

            SkillBase skill = skillObject.GetComponent<SkillBase>();
            skill.Initialize(this.weaponSuit, skillConfig);
            skillList.Add(skill);
        }

        this.skills = skillList.ToArray();
        this.isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!this.isInitialized)
        {
            // The Initialize is called in WeaponSuit.Start, which could be after this method.
            return;
        }

        foreach (SkillBase skill in this.skills)
        {
            if (skill.skillConfig.skillActivationType != SkillActivationType.Active)
            {
                continue; // Skip passive skills in the update loop.
            }

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

        if (!this.weaponSuit.capabilityController.Can(CapabilityType.CastSkill))
        {
            return;
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

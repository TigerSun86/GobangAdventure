using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    private static readonly HashSet<string> AllyWeaponTags = new HashSet<string> { Tags.PlayerWeapon, Tags.AllyTowerWeapon };
    private static readonly HashSet<string> EnemyWeaponTags = new HashSet<string> { Tags.EnemyWeapon, Tags.EnemyTowerWeapon };

    protected WeaponSuit weaponSuit;

    public SkillConfig skillConfig;

    public SkillState skillState;

    public float timeInCurrentState;

    public WeaponSuit[] targets;

    public virtual void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        this.weaponSuit = weaponSuit;
        this.skillConfig = skillConfig;
        this.skillState = SkillState.WaitingCd;
        this.timeInCurrentState = 0;
    }

    public void UpdateState()
    {
        this.timeInCurrentState += Time.fixedDeltaTime;
        switch (this.skillState)
        {
            case SkillState.WaitingCd:
                if (this.timeInCurrentState >= GetCalculatedCdTime())
                {
                    SwitchState(SkillState.SelectingTarget);
                }
                break;
            case SkillState.SelectingTarget:
                if (SelectTarget())
                {
                    SwitchState(SkillState.WaitingAct);
                }
                break;
            case SkillState.WaitingAct:
                if (!SelectTarget())
                {
                    // Lost the previous target during waiting act.
                    SwitchState(SkillState.SelectingTarget);
                }
                break;
            case SkillState.Acting:
                if (Act())
                {
                    SwitchState(SkillState.Recovering);
                }
                break;
            case SkillState.Recovering:
                if (Recover())
                {
                    SwitchState(SkillState.Completed);
                }
                break;
            case SkillState.Completed:
                // Do nothing until SkillActor triggers CD.
                break;
            default:
                Debug.LogError($"Skill state {this.skillState} is not valid to update");
                break;
        }
    }

    public bool IsWaitingAct()
    {
        return this.skillState == SkillState.WaitingAct;
    }

    public bool IsActInProgress()
    {
        return this.skillState == SkillState.Acting || this.skillState == SkillState.Recovering;
    }

    public bool IsCompleted()
    {
        return this.skillState == SkillState.Completed;
    }

    public void TriggerAction()
    {
        if (SkillState.WaitingAct == this.skillState)
        {
            SwitchState(SkillState.Acting);
        }
        else
        {
            Debug.LogError($"Skill state {this.skillState} is not valid to trigger action");
        }
    }

    public void TriggerCd()
    {
        if (SkillState.Completed != this.skillState)
        {
            Debug.LogError($"Skill state {this.skillState} is not valid to trigger cd");
            return;
        }

        if (GetCalculatedCdTime() > 0)
        {
            SwitchState(SkillState.WaitingCd);
            return;
        }

        if (SelectTarget())
        {
            SwitchState(SkillState.WaitingAct);
        }
        else
        {
            SwitchState(SkillState.SelectingTarget);
        }
    }

    public WeaponSuit[] GetTargets(float? range = null)
    {
        if (skillConfig.skillTargetConfig.maxTargets == 0)
        {
            Debug.LogError("Max targets is 0");
            return new WeaponSuit[0];
        }

        float rangeToFilter = range ?? this.skillConfig.range;
        IEnumerable<WeaponSuit> targetCandidates = GetTargetTags()
            .SelectMany(tag => GameObject.FindGameObjectsWithTag(tag))
            .Select(o => o.GetComponent<WeaponSuit>());
        IEnumerable<WeaponSuit> result = targetCandidates
                .Where(target => FilterTarget(target, rangeToFilter))
                .OrderBy(target => OrderTarget(target))
                .Take(skillConfig.skillTargetConfig.maxTargets);
        return result.ToArray();
    }

    // Returns true if finished.
    protected virtual bool Act()
    {
        throw new NotImplementedException();
    }

    // Returns true if finished.
    protected virtual bool Recover()
    {
        throw new NotImplementedException();
    }

    protected virtual bool ForceExcludeTarget(WeaponSuit target)
    {
        return false;
    }

    protected bool AreTargetsValid()
    {
        if (targets == null || targets.Length == 0)
        {
            Debug.LogError("No target to attack");
            return false;
        }

        if (targets[0].IsDestroyed())
        {
            return false;
        }

        return true;
    }

    protected bool SelectTarget()
    {
        this.targets = GetTargets();
        return this.targets.Length > 0;
    }

    private void SwitchState(SkillState skillState)
    {
        this.skillState = skillState;
        this.timeInCurrentState = 0;
    }

    private float GetCalculatedCdTime()
    {
        return this.skillConfig.cdTime - this.skillConfig.actionTime - this.skillConfig.recoveryTime;
    }

    private IEnumerable<string> GetTargetTags()
    {
        string tag = weaponSuit.transform.tag;
        bool isAlly = SkillBase.AllyWeaponTags.Contains(tag);
        bool isEnemy = SkillBase.EnemyWeaponTags.Contains(tag);

        if (!isAlly && !isEnemy)
        {
            Debug.LogError("Invalid tag");
            return Enumerable.Empty<string>();
        }

        bool targetIsAlly = skillConfig.skillTargetConfig.targetType == TargetType.Ally;
        if (isAlly == targetIsAlly)
        {
            return SkillBase.AllyWeaponTags;
        }

        return SkillBase.EnemyWeaponTags;
    }

    private float OrderTarget(WeaponSuit target)
    {
        switch (skillConfig.skillTargetConfig.targetOrdering)
        {
            case TargetOrdering.Closest:
                return Vector3.Distance(target.transform.position, weaponSuit.transform.position);
            case TargetOrdering.LowestHealth:
                throw new NotImplementedException();
            default:
                Debug.LogError($"Order type {skillConfig.skillTargetConfig.targetOrdering} not found");
                return 0;
        }
    }

    private bool FilterTarget(WeaponSuit target, float range)
    {
        if (CheckExcludedFilter(TargetFilter.All))
        {
            Debug.LogError("All targets are excluded");
            return false;
        }

        if (CheckIncludedFilter(TargetFilter.None))
        {
            Debug.LogError("None of the targets are included");
            return false;
        }

        if (ForceExcludeTarget(target))
        {
            return false;
        }

        if (range < Vector3.Distance(target.transform.position, weaponSuit.transform.position))
        {
            return false;
        }

        if (CheckExcludedFilter(TargetFilter.None)
            && CheckIncludedFilter(TargetFilter.All))
        {
            return true;
        }

        // Exclude section.
        bool isSelf = IsSelf(target);
        if (isSelf && CheckExcludedFilter(TargetFilter.Self))
        {
            return false;
        }

        WeaponLayout weaponLayout = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponLayout>();
        bool AreNeighbours = weaponLayout.AreNeighbours(this.weaponSuit, target);
        if (AreNeighbours && CheckExcludedFilter(TargetFilter.Neighbours))
        {
            return false;
        }

        // Include section.
        if (CheckIncludedFilter(TargetFilter.All))
        {
            return true;
        }

        if (isSelf && CheckIncludedFilter(TargetFilter.Self))
        {
            return true;
        }

        if (AreNeighbours && CheckIncludedFilter(TargetFilter.Neighbours))
        {
            return true;
        }

        return false;
    }

    private bool IsSelf(WeaponSuit target)
    {
        return target.gameObject == this.weaponSuit.gameObject;
    }

    private bool CheckExcludedFilter(TargetFilter targetFilter)
    {
        if (targetFilter == TargetFilter.None)
        {
            return skillConfig.skillTargetConfig.excludedTarget == TargetFilter.None;
        }

        return (skillConfig.skillTargetConfig.excludedTarget & targetFilter) == targetFilter;
    }

    private bool CheckIncludedFilter(TargetFilter targetFilter)
    {
        if (targetFilter == TargetFilter.None)
        {
            return skillConfig.skillTargetConfig.includedTarget == TargetFilter.None;
        }

        return (skillConfig.skillTargetConfig.includedTarget & targetFilter) == targetFilter;
    }
}

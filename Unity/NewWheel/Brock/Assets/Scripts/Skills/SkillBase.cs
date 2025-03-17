using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SkillBase
{
    protected GameObject owner;

    public SkillConfig skillConfig;

    public SkillState skillState;

    public float timeInCurrentState;

    public GameObject[] targets;

    public SkillBase(GameObject owner, SkillConfig skillConfig)
    {
        this.owner = owner;
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
                if (this.timeInCurrentState >= this.skillConfig.cdTime)
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
        if (SkillState.Completed == this.skillState)
        {
            SwitchState(SkillState.WaitingCd);
        }
        else
        {
            Debug.LogError($"Skill state {this.skillState} is not valid to trigger cd");
        }
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

    protected virtual bool ForceExcludeTarget(GameObject target)
    {
        return false;
    }

    private void SwitchState(SkillState skillState)
    {
        this.skillState = skillState;
        this.timeInCurrentState = 0;
    }

    private bool SelectTarget()
    {
        if (skillConfig.skillTargetConfig.maxTargets == 0)
        {
            Debug.LogError("Max targets is 0");
            return false;
        }
        IEnumerable<GameObject> targetCandidates = GameObject.FindGameObjectsWithTag(GetTargetTag());
        IEnumerable<GameObject> result = targetCandidates.Where(target => FilterTarget(target)).OrderBy(target => OrderTarget(target)).Take(skillConfig.skillTargetConfig.maxTargets);
        this.targets = result.ToArray();
        return this.targets.Length > 0;
    }

    private String GetParentTag()
    {
        return owner.transform.parent.tag;
    }

    private String GetTargetTag()
    {
        TargetType targetType = skillConfig.skillTargetConfig.targetType;
        String tag = GetParentTag();
        if (tag == "Player")
        {
            return targetType == TargetType.Ally ? "Player" : "Enemy";
        }
        else if (tag == "Enemy")
        {
            return targetType == TargetType.Ally ? "Enemy" : "Player";
        }
        else
        {
            Debug.LogError("Invalid tag");
            return null;
        }
    }

    private float OrderTarget(GameObject target)
    {
        switch (skillConfig.skillTargetConfig.targetOrdering)
        {
            case TargetOrdering.Closest:
                return Vector3.Distance(target.transform.position, owner.transform.position);
            case TargetOrdering.LowestHealth:
                throw new NotImplementedException();
            default:
                Debug.LogError($"Order type {skillConfig.skillTargetConfig.targetOrdering} not found");
                return 0;
        }
    }

    private bool FilterTarget(GameObject target)
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

        if (CheckExcludedFilter(TargetFilter.None)
            && CheckIncludedFilter(TargetFilter.All))
        {
            return true;
        }

        if (IsSelf(target))
        {
            return !CheckExcludedFilter(TargetFilter.Self)
                && CheckIncludedFilter(TargetFilter.Self);
        }

        return CheckIncludedFilter(TargetFilter.All);
    }

    private bool IsSelf(GameObject target)
    {
        return target.GetComponent<DefenceArea>().gameObject == this.owner.GetComponentInParent<DefenceArea>().gameObject;
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

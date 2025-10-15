using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    protected WeaponSuit weaponSuit;

    public SkillConfig skillConfig;

    public SkillState skillState;

    public float timeInCurrentState;

    public WeaponSuit[] targets;

    public Dictionary<SkillEvent, List<ActionBase>> eventToActions;

    public virtual void Initialize(WeaponSuit weaponSuit, SkillConfig skillConfig)
    {
        this.weaponSuit = weaponSuit;
        this.skillConfig = skillConfig;
        this.skillState = SkillState.WaitingCd;
        this.timeInCurrentState = 0;
        this.eventToActions = new Dictionary<SkillEvent, List<ActionBase>>();
        if (this.skillConfig.events != null)
        {
            foreach (KeyValuePair<SkillEvent, ActionConfig[]> kv in this.skillConfig.events)
            {
                List<ActionBase> actions = new List<ActionBase>();
                foreach (ActionConfig ac in kv.Value)
                {
                    actions.Add(ActionFactory.Create(ac, this.weaponSuit, this));
                }

                this.eventToActions[kv.Key] = actions;

                if (kv.Key == SkillEvent.SKILL_ON_ATTACK_LANDED)
                {
                    this.weaponSuit.skillActor.RegisterAttackEvents(SkillEvent.SKILL_ON_ATTACK_LANDED, actions);
                }
            }
        }

        Invoke(SkillEvent.SKILL_ON_CREATED, new SkillEventContext());
    }

    public void UpdateState()
    {
        if (EnableUpdateTimeInCurrentState())
        {
            this.timeInCurrentState += Time.fixedDeltaTime;
        }

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
                    Invoke(SkillEvent.SKILL_ON_ACTING_FINISH, new SkillEventContext());
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

    public void Invoke(SkillEvent skillEvent, SkillEventContext skillEventContext)
    {
        if (this.eventToActions.TryGetValue(skillEvent, out List<ActionBase> actions))
        {
            foreach (ActionBase action in actions)
            {
                action.Apply(skillEventContext);
            }
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
            Invoke(SkillEvent.SKILL_ON_ACTING_START, new SkillEventContext());
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

    protected virtual float GetCalculatedCdTime()
    {
        return this.skillConfig.cdTime - this.skillConfig.actionTime - this.skillConfig.recoveryTime;
    }

    protected virtual bool EnableUpdateTimeInCurrentState()
    {
        return true;
    }

    protected bool AreTargetsValid()
    {
        if (targets == null || targets.Length == 0)
        {
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
        this.targets = this.skillConfig.skillTargetConfig.GetTargets(
            this.weaponSuit, this.skillConfig.range, ForceExcludeTarget);
        return this.targets.Length > 0;
    }

    private void SwitchState(SkillState skillState)
    {
        this.skillState = skillState;
        this.timeInCurrentState = 0;
    }
}

using System;
using System.Collections.Generic;

[Serializable]
public class Modifier
{
    public ModifierConfig config;

    public float duration;

    public float interval;

    private WeaponSuit ownerWeaponSuit;

    private SkillBase ownerSkill;

    public Dictionary<SkillEvent, List<ActionBase>> eventToActions;

    public Modifier(ModifierConfig config, SkillBase ownerSkill)
    {
        this.config = config;
        this.ownerSkill = ownerSkill;
        this.duration = config.duration;
        this.interval = config.interval;
        this.eventToActions = new Dictionary<SkillEvent, List<ActionBase>>();
    }

    public void Link(WeaponSuit weaponSuit)
    {
        this.ownerWeaponSuit = weaponSuit;

        if (this.config.events != null)
        {
            foreach (KeyValuePair<SkillEvent, ActionConfig[]> kv in this.config.events)
            {
                List<ActionBase> actions = new List<ActionBase>();
                foreach (ActionConfig ac in kv.Value)
                {
                    actions.Add(ActionFactory.Create(ac, this.ownerWeaponSuit, this.ownerSkill));
                }

                this.eventToActions[kv.Key] = actions;

                if (kv.Key == SkillEvent.SKILL_ON_ATTACK_LANDED)
                {
                    this.ownerWeaponSuit.skillActor.RegisterAttackEvents(SkillEvent.SKILL_ON_ATTACK_LANDED, actions);
                }
            }
        }
    }

    public void Unlink()
    {
        if (this.eventToActions.TryGetValue(SkillEvent.SKILL_ON_ATTACK_LANDED, out List<ActionBase> actions))
        {
            this.ownerWeaponSuit.skillActor.UnregisterAttackEvents(SkillEvent.SKILL_ON_ATTACK_LANDED, actions);
        }
    }

    // Returns true if the modifier is still active.
    public bool Tick(float deltaTime)
    {
        if (!this.config.isPermanent)
        {
            // Decrease duration only if the modifier is not permanent.
            this.duration -= deltaTime;
            if (this.duration <= 0)
            {
                return false;
            }
        }

        if (this.config.interval > 0)
        {
            this.interval -= deltaTime;
            if (this.interval <= 0)
            {
                this.interval = this.config.interval;
                // TODO: Trigger interval event.
            }
        }

        return true;
    }
}
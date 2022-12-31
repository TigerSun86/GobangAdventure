using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class MainSkill : ScriptableObject
{
    [SerializeField] public MainSkillRuntimeSet activeSkills;

    [SerializeField] public MainSkillRuntimeSet inactiveSkills;

    public string skillName;

    public string description;

    public float defaultAttack;

    public float defaultCriticalRate;

    public float defaultCriticalAmount;

    public float defaultArea;

    public float defaultAttackDecrease;

    public float attack;

    public float criticalRate;

    public float criticalAmount;

    public float area;

    public float attackDecrease;

    public List<SubSkill> subSkills;

    public void Enable()
    {
        activeSkills.Add(this);
        inactiveSkills.Remove(this);
        activeSkills.NotifyChanged();
    }

    public void Disable()
    {
        activeSkills.Remove(this);
        inactiveSkills.Add(this);
        Reset();
    }

    public void Reset()
    {
        attack = defaultAttack;
        criticalRate = defaultCriticalRate;
        criticalAmount = defaultCriticalAmount;
        area = defaultArea;
        attackDecrease = defaultAttackDecrease;

        foreach (SubSkill subSkill in subSkills)
        {
            subSkill.SetMainSkill(this);
            subSkill.Reset();
        }
    }

    public IEnumerable<SubSkill> GetActiveSubSkills()
    {
        return subSkills.Where(s => s.currentLevel > 0);
    }
}
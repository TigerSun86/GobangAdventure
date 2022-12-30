using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MainSkill : ScriptableObject
{
    public bool isEnabled;

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

    public SubSkill initialSubSkill;

    public List<SubSkill> subSkills;

    public void Reset()
    {
        isEnabled = false;

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

        initialSubSkill.LevelUp();
    }
}
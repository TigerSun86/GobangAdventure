using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageData
{
    public GameObject source;

    public SkillType skillType;

    public GameObject target;

    public float rawAmount;

    public float actualAmount;

    public DamageType damageType;

    public DamageData(GameObject source, SkillType skillType, GameObject target, float rawAmount, float actualAmount, DamageType damageType)
    {
        this.source = source;
        this.skillType = skillType;
        this.target = target;
        this.rawAmount = rawAmount;
        this.actualAmount = actualAmount;
        this.damageType = damageType;
    }

    public override string ToString()
    {
        return $"Source: {source?.name}, SkillType: {skillType}, Target: {target?.name}, TargetPosition: {target?.transform.position}, RawAmount: {rawAmount}, ActualAmount: {actualAmount}, DamageType: {damageType}";
    }
}
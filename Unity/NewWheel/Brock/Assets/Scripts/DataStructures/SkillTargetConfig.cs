using System;
using UnityEngine;

[Serializable]
public class SkillTargetConfig
{
    public TargetType targetType;

    public TargetOrdering targetOrdering;

    [Range(1, 100)] public int maxTargets;

    // Excluded has a higher priority than included
    public TargetFilter excludedTarget = TargetFilter.None;

    public TargetFilter includedTarget = TargetFilter.All;

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class SkillTargetConfig
{
    private static readonly HashSet<string> ALLY_WEAPON_TAGS = new HashSet<string>
    {
        Tags.PlayerWeapon, Tags.AllyTowerWeapon
    };

    private static readonly HashSet<string> ENEMY_WEAPON_TAGS = new HashSet<string>
    {
        Tags.EnemyWeapon, Tags.EnemyTowerWeapon
    };

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
    public TargetType targetType;

    [DefaultValue(TargetOrdering.Closest)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TargetOrdering targetOrdering;

    // 0 means no limit
    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int maxTargets;

    // Excluded has a higher priority than included
    [DefaultValue(TargetFilter.None)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TargetFilter excludedTarget = TargetFilter.None;

    [DefaultValue(TargetFilter.All)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public TargetFilter includedTarget = TargetFilter.All;

    public WeaponSuit[] GetTargets(WeaponSuit owner, float range, Func<WeaponSuit, bool> forceExcludeTarget = null)
    {
        int maxTargets = this.maxTargets == 0
            ? int.MaxValue
            : this.maxTargets;

        IEnumerable<WeaponSuit> targetCandidates = GetTargetTags(owner)
            .SelectMany(tag => GameObject.FindGameObjectsWithTag(tag))
            .Select(o => o.GetComponent<WeaponSuit>());
        IEnumerable<WeaponSuit> result = targetCandidates
            .Where(target => FilterTarget(owner, target, range, forceExcludeTarget))
            .OrderBy(target => OrderTarget(owner, target))
            .Take(maxTargets);
        return result.ToArray();
    }

    public bool FilterTarget(
        WeaponSuit owner, WeaponSuit target, float range, Func<WeaponSuit, bool> forceExcludeTarget = null)
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

        if (forceExcludeTarget != null && forceExcludeTarget(target))
        {
            return false;
        }

        if (range < Vector3.Distance(target.transform.position, owner.transform.position))
        {
            return false;
        }

        if (CheckExcludedFilter(TargetFilter.None)
            && CheckIncludedFilter(TargetFilter.All))
        {
            return true;
        }

        // Exclude section.
        bool isSelf = IsSelf(owner, target);
        if (isSelf && CheckExcludedFilter(TargetFilter.Self))
        {
            return false;
        }

        // TODO: Compatible with tower layouts.
        WeaponLayout weaponLayout = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponLayout>();
        bool AreNeighbours = weaponLayout.AreNeighbours(owner, target);
        if (AreNeighbours && CheckExcludedFilter(TargetFilter.Neighbours))
        {
            return false;
        }

        Healable healable = target.weaponStand.GetComponent<Healable>();
        if (CheckExcludedFilter(TargetFilter.FullHealth)
            && healable != null
            && healable.IsFullHealth())
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

        if (CheckIncludedFilter(TargetFilter.FullHealth)
            && healable != null
            && healable.IsFullHealth())
        {
            return true;
        }

        return false;
    }

    private IEnumerable<string> GetTargetTags(WeaponSuit owner)
    {
        string tag = owner.transform.tag;
        bool isAlly = ALLY_WEAPON_TAGS.Contains(tag);
        bool isEnemy = ENEMY_WEAPON_TAGS.Contains(tag);

        if (!isAlly && !isEnemy)
        {
            Debug.LogError("Invalid tag");
            return Enumerable.Empty<string>();
        }

        bool targetIsAlly = this.targetType == TargetType.Ally;
        if (isAlly == targetIsAlly)
        {
            return ALLY_WEAPON_TAGS;
        }

        return ENEMY_WEAPON_TAGS;
    }

    private float OrderTarget(WeaponSuit owner, WeaponSuit target)
    {
        switch (this.targetOrdering)
        {
            case TargetOrdering.Closest:
                return Vector3.Distance(target.transform.position, owner.transform.position);
            case TargetOrdering.LowestHealth:
                throw new NotImplementedException();
            default:
                Debug.LogError($"Order type {this.targetOrdering} not found");
                return 0;
        }
    }

    private bool IsSelf(WeaponSuit owner, WeaponSuit target)
    {
        return target.gameObject == owner.gameObject;
    }

    private bool CheckExcludedFilter(TargetFilter targetFilter)
    {
        if (targetFilter == TargetFilter.None)
        {
            return this.excludedTarget == TargetFilter.None;
        }

        return (this.excludedTarget & targetFilter) == targetFilter;
    }

    private bool CheckIncludedFilter(TargetFilter targetFilter)
    {
        if (targetFilter == TargetFilter.None)
        {
            return this.includedTarget == TargetFilter.None;
        }

        return (this.includedTarget & targetFilter) == targetFilter;
    }

}

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public WeaponBaseType weaponBaseType;

    public int health;

    public int attack;

    public AiStrategy aiStrategy;

    public SkillConfig[] skills;
}
using UnityEngine;

[CreateAssetMenu(menuName = "SkillConfig")]
public class SkillConfig : ScriptableObject
{
    public string skillName;

    public string description;

    public SkillType skillType;

    public float value;

    public float cdTime;

    public float actionTime;

    public float recoveryTime;

    public float range;

    public SkillTargetConfig skillTargetConfig;
}

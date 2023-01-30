using UnityEngine;

public abstract class SkillPrefab : MonoBehaviour
{
    public GameObject target;

    public SkillAttributeManager skillAttributeManager;

    public abstract SkillId GetSkillId();

    public float GetSkillAttribute(AttributeType attributeType)
    {
        return skillAttributeManager.GetAttribute(GetSkillId(), attributeType);
    }
}

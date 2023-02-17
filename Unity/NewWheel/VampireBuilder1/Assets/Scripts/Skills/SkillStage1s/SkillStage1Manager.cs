using System.Collections.Generic;
using UnityEngine;

public class SkillStage1Manager : MonoBehaviour
{
    [SerializeField] SkillAttributeManager skillAttributeManager;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    [SerializeField] List<SkillId> enabledSkills;

    private Dictionary<SkillId, float> skillToTimer = new Dictionary<SkillId, float>();

    public void RefreshEnabledSkills()
    {
        enabledSkills.Clear();

        foreach (SkillId skillId in skillAttributeManager.GetAllSkills())
        {
            int level = skillAttributeManager.GetLevel(skillId);
            SkillBehaviorType? skillBehaviorType = skillAttributeManager.GetBehaviorType(skillId);
            if (level > 0 && skillBehaviorType == SkillBehaviorType.ACTIVE)
            {
                enabledSkills.Add(skillId);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (SkillId skillId in enabledSkills)
        {
            if (UpdateSkillTimer(skillId))
            {
                InitiateSkill(skillId);
            }
        }
    }

    private void InitiateSkill(SkillId skillId)
    {
        GameObject prefab = skillIdToPrefab[skillId];

        Transform playerTransform = Manager.instance.PlayerTransform;
        Vector3 position = new Vector3();
        position.x = playerTransform.position.x + (playerTransform.localScale.x / 2) + (prefab.transform.localScale.x / 2);
        position.y = playerTransform.position.y;

        GameObject gameObject = Instantiate(prefab, position, Quaternion.identity, this.transform);

        SkillStage1 skillStage1 = gameObject.GetComponent<SkillStage1>();
        skillStage1.skillId = skillId;

        Move move = gameObject.GetComponent<Move>();
        FloatVariable speed = ScriptableObject.CreateInstance<FloatVariable>();
        speed.SetValue(skillAttributeManager.GetAttribute(skillId, AttributeType.SPEED));
        move.defaultSpeed = speed;
    }

    private bool UpdateSkillTimer(SkillId skillId)
    {
        if (!skillToTimer.ContainsKey(skillId))
        {
            skillToTimer[skillId] = 0f;
        }

        float timer = skillToTimer[skillId];
        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
        {
            skillToTimer[skillId] = timer;
            return false;
        }

        float cd = skillAttributeManager.GetAttribute(skillId, AttributeType.CD);
        skillToTimer[skillId] = cd;

        return true;
    }
}

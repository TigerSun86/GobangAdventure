using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillAttributeManager skillAttributeManager;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    [SerializeField] List<SkillId> enabledSkills;

    private bool isFirstShot;

    public void RefreshEnabledSkills()
    {
        enabledSkills.Clear();

        foreach (SkillId skillId in skillAttributeManager.GetAllSkills())
        {
            int level = skillAttributeManager.GetLevel(skillId);
            if (level > 0)
            {
                enabledSkills.Add(skillId);
            }
        }
    }

    public void InstantiateSkillPrefabs(Collider2D other, GameObject bullet)
    {
        if (isFirstShot)
        {
            isFirstShot = false;
            RefreshEnabledSkills();
        }

        foreach (SkillId skillId in enabledSkills)
        {
            if (!skillIdToPrefab.ContainsKey(skillId))
            {
                continue;
            }

            GameObject prefab = skillIdToPrefab[skillId];
            GameObject instance = Instantiate(
                prefab,
                other.gameObject.transform.position,
                Quaternion.identity,
                this.transform);
            SkillPrefab skillPrefab = instance.GetComponent<SkillPrefab>();
            skillPrefab.target = other.gameObject;
            skillPrefab.skillAttributeManager = skillAttributeManager;
        }
    }

    private void Start()
    {
        isFirstShot = true;
    }
}

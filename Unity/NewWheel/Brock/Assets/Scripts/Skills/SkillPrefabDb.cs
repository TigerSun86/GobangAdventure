using System;
using UnityEngine;

public class SkillPrefabDb : MonoBehaviour
{
    public static SkillPrefabDb Instance { get; private set; }

    [SerializeField]
    public SkillTypeToGameObjectDictionary skillPrefabs;

    public GameObject GetSkillPrefab(SkillType skillType)
    {
        if (skillPrefabs.TryGetValue(skillType, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogError($"Skill prefab not found for type: {skillType}");
        return null;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

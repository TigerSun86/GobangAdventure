using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyConfigDb
{
    [SerializeField]
    private StringToEnemyConfigDictionary enemyConfigMap;

    public EnemyConfigDb(List<EnemyConfig> enemyConfigs)
    {
        this.enemyConfigMap = new StringToEnemyConfigDictionary();
        foreach (EnemyConfig enemy in enemyConfigs)
        {
            string key = enemy.enemyId;
            if (this.enemyConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate enemy config found: {key}. Skipping.");
                continue;
            }

            this.enemyConfigMap.Add(key, enemy);
        }
    }

    public EnemyConfig Get(string id)
    {
        if (enemyConfigMap.TryGetValue(id, out EnemyConfig skill))
        {
            return skill;
        }

        Debug.LogWarning($"enemy config not found for id: {id}");
        return null;
    }

    public IEnumerable<EnemyConfig> GetAll()
    {
        return enemyConfigMap.Values;
    }
}

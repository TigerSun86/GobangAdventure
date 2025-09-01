using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemConfigDb
{
    [SerializeField]
    private StringToItemConfigDictionary itemConfigMap;

    public ItemConfigDb(List<ItemConfig> itemConfigs)
    {
        this.itemConfigMap = new StringToItemConfigDictionary();
        foreach (ItemConfig item in itemConfigs)
        {
            string key = item.GetId();
            if (this.itemConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate item config found: {key}. Skipping.");
                continue;
            }

            this.itemConfigMap.Add(key, item);
        }
    }

    public ItemConfig Get(string id)
    {
        if (itemConfigMap.TryGetValue(id, out ItemConfig skill))
        {
            return skill;
        }

        Debug.LogWarning($"Item config not found for id: {id}");
        return null;
    }

    public IEnumerable<ItemConfig> GetAll()
    {
        return itemConfigMap.Values;
    }
}

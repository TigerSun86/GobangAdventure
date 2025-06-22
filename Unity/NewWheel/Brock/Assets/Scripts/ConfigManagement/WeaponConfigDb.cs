using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponConfigDb
{
    [SerializeField]
    private StringToWeaponConfigDictionary weaponConfigMap;

    public WeaponConfigDb(List<WeaponConfig> weaponConfigs)
    {
        this.weaponConfigMap = new StringToWeaponConfigDictionary();
        foreach (WeaponConfig weapon in weaponConfigs)
        {
            string key = weapon.GetId();
            if (this.weaponConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate weapon config found: {key}. Skipping.");
                continue;
            }

            this.weaponConfigMap.Add(key, weapon);
        }
    }

    public WeaponConfig Get(string id)
    {
        if (weaponConfigMap.TryGetValue(id, out WeaponConfig skill))
        {
            return skill;
        }

        Debug.LogWarning($"Weapon config not found for id: {id}");
        return null;
    }

    public IEnumerable<WeaponConfig> GetAll()
    {
        return weaponConfigMap.Values;
    }
}

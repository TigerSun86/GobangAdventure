using System.Collections.Generic;
using UnityEngine;

public class WeaponConfigDb
{
    private Dictionary<string, WeaponConfig2> weaponConfigMap;

    public WeaponConfigDb(List<WeaponConfig2> weaponConfigs)
    {
        this.weaponConfigMap = new Dictionary<string, WeaponConfig2>();
        foreach (WeaponConfig2 weapon in weaponConfigs)
        {
            string key = weapon.weaponName + weapon.level;
            if (this.weaponConfigMap.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate weapon config found: {key}. Skipping.");
                continue;
            }

            this.weaponConfigMap.Add(key, weapon);
        }
    }

    public WeaponConfig2 Get(string name, int level)
    {
        return Get(name + level);
    }

    public WeaponConfig2 Get(string id)
    {
        if (weaponConfigMap.TryGetValue(id, out WeaponConfig2 skill))
        {
            return skill;
        }

        Debug.LogWarning($"Weapon config not found for id: {id}");
        return null;
    }

    public IEnumerable<WeaponConfig2> GetAll()
    {
        return weaponConfigMap.Values;
    }
}

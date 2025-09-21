using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    [SerializeField, AssignedInCode]
    private float goldCount = 0;

    [SerializeField, AssignedInCode]
    private float weaponCount = 0;

    [SerializeField, AssignedInCode]
    private float itemCount = 0;

    [SerializeField, Required]
    private GameObject goldLootPrefab;

    [SerializeField, Required]
    private GameObject weaponLootPrefab;

    [SerializeField, Required]
    private GameObject itemLootPrefab;

    [SerializeField]
    private UnityEvent onChangeMoney;

    public void GenerateLoot(LootConfig lootConfig, Vector3 position)
    {
        // Prepare a list of loot prefabs and their corresponding pickup actions
        List<(GameObject prefab, Action onPickup)> lootToSpawn = new List<(GameObject prefab, Action onPickup)>();

        if (lootConfig.goldDropRate > 0 && lootConfig.goldDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.goldLootPrefab, () => { IncreaseGoldCount(); }));
        }

        if (lootConfig.weaponDropRate > 0 && lootConfig.weaponDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.weaponLootPrefab, () => { IncreaseWeaponCount(); }));
        }

        if (lootConfig.itemDropRate > 0 && lootConfig.itemDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.itemLootPrefab, () => { IncreaseItemCount(); }));
        }

        float offsetRadius = 0.6f; // Distance from center for extra loot
        for (int i = 0; i < lootToSpawn.Count; i++)
        {
            Vector3 spawnPos = position;
            if (i == 0)
            {
                // First loot at the original position
                spawnPos = position;
            }
            else
            {
                // Spawn loot at a random position nearby the original position, avoiding overlap (2D: x/y only)
                float randomRadius = offsetRadius + UnityEngine.Random.Range(-0.1f, 0.1f);
                float randomAngle = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
                spawnPos = position + new Vector3(
                    Mathf.Cos(randomAngle),
                    Mathf.Sin(randomAngle),
                    0
                ) * randomRadius;
            }

            GameObject lootObj = Instantiate(lootToSpawn[i].prefab, spawnPos, Quaternion.identity, this.transform);
            Loot loot = lootObj.GetComponent<Loot>();
            loot.OnPickup += lootToSpawn[i].onPickup;
        }
    }

    // Helper methods for goldCount
    public int GetGoldCount()
    {
        return (int)this.goldCount;
    }

    public void IncreaseGoldCount(float amount = 1f)
    {
        this.goldCount += amount;
        this.onChangeMoney.Invoke();
    }

    public void DecreaseGoldCount(float amount = 1f)
    {
        this.goldCount -= amount;
        this.onChangeMoney.Invoke();
    }

    // Helper methods for weaponCount
    public int GetWeaponCount()
    {
        return (int)this.weaponCount;
    }

    public void IncreaseWeaponCount(float amount = 1f)
    {
        this.weaponCount += amount;
        this.onChangeMoney.Invoke();
    }

    public void DecreaseWeaponCount(float amount = 1f)
    {
        this.weaponCount -= amount;
        this.onChangeMoney.Invoke();
    }

    // Helper methods for itemCount
    public int GetItemCount()
    {
        return (int)this.itemCount;
    }

    public void IncreaseItemCount(float amount = 1f)
    {
        this.itemCount += amount;
        this.onChangeMoney.Invoke();
    }

    public void DecreaseItemCount(float amount = 1f)
    {
        this.itemCount -= amount;
        this.onChangeMoney.Invoke();
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

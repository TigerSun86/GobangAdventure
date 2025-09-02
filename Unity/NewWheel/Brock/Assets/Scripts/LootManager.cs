using System;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }

    public float goldCount = 0;

    public float weaponCount = 0;

    public float itemCount = 0;

    [SerializeField, Required]
    private GameObject goldLootPrefab;

    [SerializeField, Required]
    private GameObject weaponLootPrefab;

    [SerializeField, Required]
    private GameObject itemLootPrefab;

    public void GenerateLoot(LootConfig lootConfig, Vector3 position)
    {
        // Prepare a list of loot prefabs and their corresponding pickup actions
        List<(GameObject prefab, Action onPickup)> lootToSpawn = new List<(GameObject prefab, Action onPickup)>();

        if (lootConfig.goldDropRate > 0 && lootConfig.goldDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.goldLootPrefab, () => { this.goldCount += 1f; }));
        }

        if (lootConfig.weaponDropRate > 0 && lootConfig.weaponDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.weaponLootPrefab, () => { this.weaponCount += 1f; }));
        }

        if (lootConfig.itemDropRate > 0 && lootConfig.itemDropRate >= UnityEngine.Random.Range(0f, 1f))
        {
            lootToSpawn.Add((this.itemLootPrefab, () => { this.itemCount += 1f; }));
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<SpawnWaveData> spawnWaveConfigs;

    [SerializeField] SpawnWaveData currentWaveConfig;

    // Unity does not support SerializeField of Dictionary.
    [SerializeField] List<EnemyTypeAndPrefab> enemyTypeAndPrefabMapping;

    [SerializeField] Vector2RuntimeSet spawnPositions;

    [SerializeField] float spawnInterval;

    [SerializeField] IntVariable spawnWaveNumber;

    [SerializeField] IntVariable normalEnemyMaxHealth;

    private float timer;

    private Dictionary<EnemyType, GameObject> enemyPrefabs;

    private void Awake()
    {
        enemyPrefabs = getEnemyPrefabDictionary();
    }

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer < 0f)
        {
            SpawnEnemies();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemies()
    {
        spawnWaveNumber.ApplyChange(1);
        if (spawnWaveNumber.value % 5 == 0)
        {
            normalEnemyMaxHealth.value += 5;
        }

        UpdateCurrentWaveConfig();

        List<Vector2> positionCandidates = new List<Vector2>(spawnPositions.Items);

        foreach (SpawnData spawnData in currentWaveConfig.enemies)
        {
            if (!positionCandidates.Any())
            {
                break;
            }

            if (!spawnData.isValid())
            {
                Debug.LogError($"Invalid spawn data {spawnData}");
                continue;
            }

            GameObject enemyPrefab = enemyPrefabs[spawnData.enemyType];
            if (enemyPrefab == null)
            {
                Debug.LogError($"Unregistered enemy type {spawnData.enemyType}");
                continue;
            }

            int spawnCount = Random.Range(spawnData.minCount, spawnData.maxCount + 1);
            for (int i = 0; i < spawnCount; i++)
            {
                if (!positionCandidates.Any())
                {
                    break;
                }

                int randomPositionIndex = Random.Range(0, positionCandidates.Count);
                Vector2 position = positionCandidates[randomPositionIndex];
                positionCandidates.RemoveAt(randomPositionIndex);

                GameObject enemyObject = Instantiate(enemyPrefab);
                enemyObject.transform.position = position;
            }
        }
    }

    // Update current wave config at runtime in order to change and test configs in Unity editor.
    private void UpdateCurrentWaveConfig()
    {
        if (spawnWaveConfigs == null || !spawnWaveConfigs.Any())
        {
            Debug.LogError("Could not find spawn wave config");
        }

        bool hasDuplicatedWaves = spawnWaveConfigs
            .GroupBy(o => o.waveNumber)
            .Where(g => g.Count() > 1)
            .Any();
        if (hasDuplicatedWaves)
        {
            // This happens when manually add config in Unity editor in runtime.
            // Just silently wait for the manual change.
            return;
        }

        List<SpawnWaveData> configs = new List<SpawnWaveData>(spawnWaveConfigs);
        Comparer<SpawnWaveData> comparer = Comparer<SpawnWaveData>.Create((a, b) => a.waveNumber - b.waveNumber);
        configs.Sort(comparer);

        // Use the last wave config that its number is less than current wave number.
        // If the first wave config's number is greater than current wave number,
        // Then use the first wave config.
        int index = configs.BinarySearch(new SpawnWaveData(spawnWaveNumber.value), comparer);
        if (index < 0)
        {
            index = ~index;
            if (index > 0)
            {
                index--;
            }
        }

        currentWaveConfig = configs[index];
    }

    private Dictionary<EnemyType, GameObject> getEnemyPrefabDictionary()
    {
        return enemyTypeAndPrefabMapping
            .ToDictionary(o => o.enemyType, o => o.prefab);
    }
}

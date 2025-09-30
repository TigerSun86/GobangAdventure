using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField, Required]
    private GameObject enemyPrefab;

    [SerializeField, AssignedInCode]
    private WaveConfigDb waveConfigDb;

    [SerializeField, Required]
    private GameObject spawnPointMiddle;

    [SerializeField, Required]
    private GameObject spawnPointDown;

    private float spawnTimer;

    private EnemyInWaveConfig[] currentWaveEnemyConfigs;

    private float[] enemySpawnTimers;

    private Dictionary<Enemy, bool> currentWaveBosses;

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

    public void OnEnemyDie(Enemy enemy)
    {
        if (this.currentWaveBosses.ContainsKey(enemy))
        {
            this.currentWaveBosses[enemy] = false; // Mark this boss as defeated
        }

        if (this.currentWaveBosses.Count > 0 && !this.currentWaveBosses.Any(kv => kv.Value))
        {
            // All bosses defeated
            WaveManager.Instance.CompleteWaveEarly();
        }
    }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            this.waveConfigDb = ConfigDb.Instance.waveConfigDb;
            this.spawnTimer = 0f;

            this.currentWaveEnemyConfigs = this.waveConfigDb.GetWaveConfig(WaveManager.Instance.currentWave - 1).enemyInWaveConfigs;
            this.enemySpawnTimers = new float[this.currentWaveEnemyConfigs.Length];
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }

        // Reset boss count at the start of each wave.
        Instance.currentWaveBosses = new Dictionary<Enemy, bool>();
    }

    private void FixedUpdate()
    {
        if (!WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        this.spawnTimer += Time.fixedDeltaTime;

        for (int i = 0; i < this.currentWaveEnemyConfigs.Length; i++)
        {
            EnemyInWaveConfig enemyInWaveConfig = currentWaveEnemyConfigs[i];
            if (this.spawnTimer < enemyInWaveConfig.spawnDelay)
            {
                continue;
            }

            // Check if it's time to spawn the next enemy in this slot
            if (enemySpawnTimers[i] <= 0f)
            {
                SpawnEnemies(enemyInWaveConfig);

                this.enemySpawnTimers[i] = enemyInWaveConfig.spawnInterval;
            }
            else
            {
                this.enemySpawnTimers[i] -= Time.fixedDeltaTime;
            }
        }
    }

    private void SpawnEnemies(EnemyInWaveConfig enemyInWaveConfig)
    {
        Vector3 enemyPosition = GetSpawnPosition(enemyInWaveConfig.spawnPoint) + (Vector3)enemyInWaveConfig.positionInFleet;
        GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, this.transform);
        enemyObject.GetComponent<Enemy>().Initialize(enemyInWaveConfig.enemyConfig);
        if (enemyInWaveConfig.isBoss)
        {
            this.currentWaveBosses[enemyObject.GetComponent<Enemy>()] = true;
        }
    }

    private Vector3 GetSpawnPosition(SpawnPoint spawnPoint)
    {
        switch (spawnPoint)
        {
            case SpawnPoint.Middle:
                return spawnPointMiddle.transform.position;
            case SpawnPoint.Down:
                return spawnPointDown.transform.position;
            default:
                Debug.LogError($"Unknown spawn point: {spawnPoint}");
                return Vector3.zero;
        }
    }
}

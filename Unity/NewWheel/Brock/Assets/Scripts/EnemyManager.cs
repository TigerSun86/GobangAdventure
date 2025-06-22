using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField] WaveConfig[] waveConfigs;

    [SerializeField] GameObject enemyPrefab;

    [SerializeField, AssignedInCode]
    private EnemyConfigDb enemyConfigDb;


    public bool hasSpawnedEnemies;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            this.hasSpawnedEnemies = false;
            this.enemyConfigDb = ConfigDb.Instance.enemyConfigDb;
        }
    }

    private void FixedUpdate()
    {
        if (!WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        int enemyCount = GetComponentsInChildren<Enemy>().Length;
        if (enemyCount <= 0)
        {
            if (WaveManager.Instance.currentWave >= this.enemyConfigDb.GetWaveCount())
            {
                Debug.LogError("Wave config has not setup for wave " + WaveManager.Instance.currentWave);
                return;
            }

            if (this.hasSpawnedEnemies)
            {
                this.hasSpawnedEnemies = false;
                WaveManager.Instance.WaveCompleted();
            }
            else
            {
                this.hasSpawnedEnemies = true;
                SpawnEnemies();
            }
        }
    }

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            Destroy(enemy.gameObject);
        }
    }

    private void SpawnEnemies()
    {
        Vector3 fleetPosition = new Vector3(Random.Range(6, 8), Random.Range(-2, 2), 0);
        EnemyInFleetConfig[] enemyInFleetConfigs = this.enemyConfigDb.GetWaveConfig(WaveManager.Instance.currentWave - 1).enemyInFleetConfigs;
        foreach (EnemyInFleetConfig enemyInFleetConfig in enemyInFleetConfigs)
        {
            Vector3 enemyPosition = fleetPosition + (Vector3)enemyInFleetConfig.positionInFleet;
            GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, this.transform);
            enemyObject.GetComponent<Enemy>().SetWeapon(enemyInFleetConfig.enemyConfig.weaponConfig2);
            enemyObject.GetComponent<Enemy>().aiStrategy = enemyInFleetConfig.enemyConfig.aiStrategy;
        }
    }
}

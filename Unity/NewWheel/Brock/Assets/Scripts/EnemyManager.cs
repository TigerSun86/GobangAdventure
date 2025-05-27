using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] WaveConfig[] waveConfigs;

    [SerializeField] GameObject enemyPrefab;

    public static EnemyManager Instance { get; private set; }

    public int fleetIndex;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            this.fleetIndex = 0;
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
            if (WaveManager.Instance.currentWave > waveConfigs.Length)
            {
                Debug.LogError("Wave config has not setup for wave " + WaveManager.Instance.currentWave);
                return;
            }

            if (this.fleetIndex >= waveConfigs[WaveManager.Instance.currentWave - 1].fleetConfigs.Length)
            {
                this.fleetIndex = 0;
                WaveManager.Instance.WaveCompleted();
            }
            else
            {
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
        FleetConfig[] fleetConfigs = waveConfigs[WaveManager.Instance.currentWave - 1].fleetConfigs;
        foreach (EnemyInFleetConfig enemyInFleetConfig in fleetConfigs[this.fleetIndex].enemyInFleetConfig)
        {
            Vector3 enemyPosition = fleetPosition + (Vector3)enemyInFleetConfig.positionInFleet;
            GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, this.transform);
            enemyObject.GetComponent<Enemy>().SetWeapon(enemyInFleetConfig.enemyConfig.weaponConfig);
            enemyObject.GetComponent<Enemy>().aiStrategy = enemyInFleetConfig.enemyConfig.aiStrategy;
        }

        this.fleetIndex++;
    }
}

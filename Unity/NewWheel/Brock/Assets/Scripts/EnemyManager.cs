using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] FleetConfig fleetConfig;

    [SerializeField] GameObject enemyPrefab;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    [SerializeField] int maxEnemyCount;

    public static EnemyManager Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void FixedUpdate()
    {
        if (!WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        int enemyCount = GetComponentsInChildren<Enemy>().Length;
        if (enemyCount < maxEnemyCount)
        {
            SpawnEnemies();
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
        Vector3 fleetPosition = new Vector3(Random.Range(0, 10), Random.Range(-2, 2), 0);
        foreach (EnemyConfig enemyConfig in fleetConfig.enemyConfigs)
        {
            Vector3 enemyPosition = fleetPosition + (Vector3)enemyConfig.positionInFleet;
            GameObject enemyObject = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity, this.transform);
            GameObject weaponPrefab = skillIdToPrefab[enemyConfig.weaponBaseType];
            enemyObject.GetComponent<Enemy>().SetWeapon(weaponPrefab);
        }
    }
}

using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] GameObject[] weaponPrefabs;

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
        Vector3 position = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
        GameObject enemyObject = Instantiate(enemyPrefab, position, Quaternion.identity, this.transform);
        enemyObject.GetComponent<Enemy>().SetWeapon(weaponPrefabs[Random.Range(0, weaponPrefabs.Length)]);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [SerializeField] int maxWaveTime;

    public static WaveManager Instance { get; private set; }

    public bool IsWaveRunning { get; private set; }

    public int currentWave = 0;

    public int currentWaveTime = 0;

    public bool isEarlyComplete;

    private WeaponInventory weaponInventory;

    private string[] hackyTestWeapons = { "Stun Rock 1", "Attack Buff Scissor 1" };

    private int hackyTestWeaponsIndex = 0;

    public void WaveCompleted()
    {
        this.IsWaveRunning = false;
        if (EnemyManager.Instance.gameObject.activeInHierarchy)
        {
            EnemyManager.Instance.DestroyAllEnemies();
        }

        StopAllCoroutines();
        SceneUtility.LoadShopScene();
    }

    public void CompleteWaveEarly()
    {
        if (this.currentWaveTime > 10)
        {
            this.currentWaveTime = 10;
            this.isEarlyComplete = true;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this.IsWaveRunning = SceneUtility.IsWaveSceneActive();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneUtility.IsWaveSceneActive())
        {
            StartNewWave();
        }
    }

    private void StartNewWave()
    {
        StopAllCoroutines();
        this.currentWave++;
        this.currentWaveTime = this.maxWaveTime;
        this.IsWaveRunning = true;
        this.isEarlyComplete = false;
        StartCoroutine(WaveTimer());
    }

    private IEnumerator WaveTimer()
    {
        while (this.currentWaveTime > 0)
        {
            yield return new WaitForSeconds(1);
            this.currentWaveTime--;
        }

        WaveCompleted();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            WaveCompleted();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (this.weaponInventory == null)
            {
                this.weaponInventory = ConfigDb.Instance.weaponInventory;
            }

            this.weaponInventory.TryAdd(this.hackyTestWeapons[this.hackyTestWeaponsIndex]);
            this.hackyTestWeaponsIndex = (this.hackyTestWeaponsIndex + 1) % this.hackyTestWeapons.Length;

            GameObject.Find("Player").GetComponent<Player>().RefreshWeapons();
        }
    }
}

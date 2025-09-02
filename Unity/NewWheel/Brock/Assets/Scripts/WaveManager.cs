using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [SerializeField] int maxWaveTime;

    [SerializeField, AssignedInCode]
    private MoneyManager moneyManager;

    public static WaveManager Instance { get; private set; }

    public bool IsWaveRunning { get; private set; }

    public int currentWave = 0;

    public int currentWaveTime = 0;

    private WeaponInventory weaponInventory;

    public void WaveCompleted()
    {
        IsWaveRunning = false;
        if (EnemyManager.Instance.gameObject.activeInHierarchy)
        {
            EnemyManager.Instance.DestroyAllEnemies();
        }

        StopAllCoroutines();
        if (this.moneyManager == null)
        {
            this.moneyManager = MoneyManager.Instance;
        }
        this.moneyManager.IncreaseCountToBuy();
        SceneUtility.LoadShopScene();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            IsWaveRunning = SceneUtility.IsWaveSceneActive();
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
        currentWave++;
        currentWaveTime = maxWaveTime;
        IsWaveRunning = true;
        StartCoroutine(WaveTimer());
    }

    private IEnumerator WaveTimer()
    {
        while (currentWaveTime > 0)
        {
            yield return new WaitForSeconds(1);
            currentWaveTime--;
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

            if (s)
            {
                this.weaponInventory.TryAdd("Basic Scissor 1");
                s = false;
            }
            else
            {
                this.weaponInventory.TryAdd("Attack Buff Scissor 1");

                s = true;
            }

            GameObject.Find("Player").GetComponent<Player>().RefreshWeapons();
        }
    }
    private bool s = false;
}

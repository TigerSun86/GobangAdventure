using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [SerializeField] int maxWaveTime;
    [SerializeField] ItemDb itemDb;

    public static WaveManager Instance { get; private set; }

    public bool IsWaveRunning { get; private set; }

    public int currentWave = 0;

    public int currentWaveTime = 0;

    public void WaveCompleted()
    {
        IsWaveRunning = false;
        EnemyManager.Instance.DestroyAllEnemies();
        StopAllCoroutines();
        this.itemDb.IncreaseCountToBuy();
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
            itemDb.playerItemNames.Add("BasicRock1");
            itemDb.playerItemNames.Add("BasicPaper1");
            itemDb.playerItemNames.Add("BasicScissor1");

            GameObject.Find("Player").GetComponent<Player>().InitializeWeapons();
        }
    }
}

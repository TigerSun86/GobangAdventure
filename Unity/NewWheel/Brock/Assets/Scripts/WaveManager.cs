using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] int maxWaveTime;
    [SerializeField] ItemDb itemDb;

    public static WaveManager Instance { get; private set; }

    public bool IsWaveRunning { get; private set; }

    public static int currentWave = 0;

    public int currentWaveTime = 0;

    public void WaveCompleted()
    {
        IsWaveRunning = false;
        EnemyManager.Instance.DestroyAllEnemies();
        StopAllCoroutines();
        SceneUtility.LoadShopScene();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartNewWave();
    }

    private void StartNewWave()
    {
        StopAllCoroutines();
        currentWave++;
        waveText.text = "Wave: " + currentWave;
        currentWaveTime = maxWaveTime;
        IsWaveRunning = true;
        StartCoroutine(WaveTimer());
    }

    private IEnumerator WaveTimer()
    {
        while (currentWaveTime > 0)
        {
            timerText.text = currentWaveTime.ToString();
            yield return new WaitForSeconds(1);
            currentWaveTime--;
        }

        timerText.text = "0";
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

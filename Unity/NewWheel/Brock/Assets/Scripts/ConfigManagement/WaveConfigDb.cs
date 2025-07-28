using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WaveConfigDb
{
    [SerializeField]
    private WaveConfig[] waveConfigs;

    public WaveConfigDb(List<EnemyInWaveConfig> waveConfigs)
    {
        this.waveConfigs = CreateWaveConfigs(waveConfigs);
    }

    public WaveConfig GetWaveConfig(int waveId)
    {
        if (waveId < 0 || waveId >= this.waveConfigs.Length)
        {
            Debug.LogWarning($"Wave config not found for id: {waveId}");
            return null;
        }

        return this.waveConfigs[waveId];
    }

    public int GetWaveCount()
    {
        return this.waveConfigs.Length;
    }

    private WaveConfig[] CreateWaveConfigs(List<EnemyInWaveConfig> waveConfigs)
    {
        List<List<EnemyInWaveConfig>> waveConfigList = new List<List<EnemyInWaveConfig>>();
        int currentWaveId = -1;
        foreach (EnemyInWaveConfig waveConfig in waveConfigs)
        {
            if (waveConfig.waveId < 0 || waveConfig.waveId < currentWaveId)
            {
                Debug.LogWarning($"Invalid wave config found, wave: {waveConfig.waveId}, current wave: {currentWaveId}. Skipping.");
                continue;
            }

            if (waveConfig.waveId > currentWaveId)
            {
                currentWaveId = waveConfig.waveId;
                waveConfigList.Add(new List<EnemyInWaveConfig>());
            }

            waveConfigList[currentWaveId].Add(waveConfig);
        }

        return waveConfigList.Select(waveConfig =>
        {
            WaveConfig config = new WaveConfig();
            config.enemyInWaveConfigs = waveConfig.ToArray();
            return config;
        }).ToArray();
    }
}

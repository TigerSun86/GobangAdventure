using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyConfigDb
{
    [SerializeField]
    private WaveConfig[] waveConfigs;

    public EnemyConfigDb(List<RawEnemyConfig> enemyConfigs)
    {
        this.waveConfigs = CreateWaveConfigs(enemyConfigs);
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

    private WaveConfig[] CreateWaveConfigs(List<RawEnemyConfig> enemyConfigs)
    {
        Dictionary<int, Dictionary<int, EnemyInFleetConfig>> waveIdToEnemyIdToConfig = new Dictionary<int, Dictionary<int, EnemyInFleetConfig>>();
        foreach (RawEnemyConfig enemyConfig in enemyConfigs)
        {
            if (enemyConfig.waveId < 0 || enemyConfig.enemyId < 0)
            {
                Debug.LogWarning($"Invalid enemy config found, wave: {enemyConfig.waveId}, enemy: {enemyConfig.enemyId}. Skipping.");
                continue;
            }

            Dictionary<int, EnemyInFleetConfig> waveConfig = waveIdToEnemyIdToConfig.GetValueOrDefault(enemyConfig.waveId);
            if (waveConfig == null)
            {
                waveConfig = new Dictionary<int, EnemyInFleetConfig>();
                waveIdToEnemyIdToConfig[enemyConfig.waveId] = waveConfig;
            }

            if (waveConfig.ContainsKey(enemyConfig.enemyId))
            {
                Debug.LogWarning($"Duplicate enemy config found, wave: {enemyConfig.waveId}, enemy: {enemyConfig.enemyId}. Skipping.");
                continue;
            }

            waveConfig[enemyConfig.enemyId] = enemyConfig.enemyInFleetConfig;
        }

        List<WaveConfig> waveConfigList = new List<WaveConfig>();
        for (int waveId = 0; waveId < waveIdToEnemyIdToConfig.Count; waveId++)
        {
            if (!waveIdToEnemyIdToConfig.TryGetValue(waveId, out Dictionary<int, EnemyInFleetConfig> enemyInFleetConfigs))
            {
                Debug.LogWarning($"No enemy configs found for wave: {waveId}. Skipping.");
                continue;
            }

            WaveConfig waveConfig = ScriptableObject.CreateInstance<WaveConfig>();
            waveConfig.enemyInFleetConfigs = new EnemyInFleetConfig[enemyInFleetConfigs.Count];
            for (int enemyId = 0; enemyId < enemyInFleetConfigs.Count; enemyId++)
            {
                if (!enemyInFleetConfigs.TryGetValue(enemyId, out EnemyInFleetConfig enemyInFleetConfig))
                {
                    Debug.LogWarning($"No enemy config found for wave: {waveId}, enemy: {enemyId}. Skipping.");
                    continue;
                }

                waveConfig.enemyInFleetConfigs[enemyId] = enemyInFleetConfig;
            }

            waveConfigList.Add(waveConfig);
        }

        return waveConfigList.ToArray();
    }
}

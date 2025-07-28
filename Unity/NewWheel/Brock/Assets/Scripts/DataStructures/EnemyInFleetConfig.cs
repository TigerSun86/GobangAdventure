using System;
using UnityEngine;

[Serializable]
public class EnemyInWaveConfig
{
    public int waveId;

    public EnemyConfig enemyConfig;

    public SpawnPoint spawnPoint;

    public Vector2 positionInFleet;

    public float spawnDelay;

    public float spawnInterval;
}
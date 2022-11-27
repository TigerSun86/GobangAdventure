using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnWaveData
{
    public int waveNumber;

    public List<SpawnData> enemies;

    public SpawnWaveData(int waveNumber)
    {
        this.waveNumber = waveNumber;
    }
}
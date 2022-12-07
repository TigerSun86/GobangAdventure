using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalHitLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] float changeAmount = 0.2f;

    [SerializeField] FloatVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount);
    }
}

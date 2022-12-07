using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseAttackIntervalLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] float changeAmount = -0.1f;

    [SerializeField] FloatVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount);
    }
}

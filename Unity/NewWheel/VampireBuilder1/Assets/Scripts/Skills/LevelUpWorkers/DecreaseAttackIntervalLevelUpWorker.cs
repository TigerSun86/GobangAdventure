using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseAttackIntervalLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] FloatVariable changeAmount;

    [SerializeField] FloatVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount.value);
    }
}

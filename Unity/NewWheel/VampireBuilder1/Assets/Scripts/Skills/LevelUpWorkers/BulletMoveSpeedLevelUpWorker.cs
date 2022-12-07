using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMoveSpeedLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] float changeAmount = 1f;

    [SerializeField] FloatVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount);
    }
}

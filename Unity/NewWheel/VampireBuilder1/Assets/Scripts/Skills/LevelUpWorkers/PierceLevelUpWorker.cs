using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] int changeAmount = 1;

    [SerializeField] IntVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount);
    }
}

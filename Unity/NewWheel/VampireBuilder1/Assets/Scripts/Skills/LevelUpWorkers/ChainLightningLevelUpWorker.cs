using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] GameObject prefab;

    [SerializeField] int changeAmount = 1;

    [SerializeField] IntVariable factor;

    bool isEnabled = false;

    public void LevelUp(int newLevel)
    {
        if (newLevel == 1)
        {
            isEnabled = true;
        }
        else
        {
            factor.ApplyChange(changeAmount);
        }
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (!isEnabled)
        {
            return;
        }

        Instantiate(
            prefab,
            other.gameObject.transform.position,
            Quaternion.identity,
            this.transform);
    }
}

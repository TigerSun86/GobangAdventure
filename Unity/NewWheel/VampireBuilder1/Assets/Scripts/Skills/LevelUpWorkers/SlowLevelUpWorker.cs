using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] GameObject prefab;

    [SerializeField] FloatVariable changeAmount;

    [SerializeField] FloatVariable factor;

    public void LevelUp(int newLevel)
    {
        factor.ApplyChange(changeAmount);
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (factor.value >= 1f)
        {
            return;
        }

        Instantiate(
            prefab,
            other.gameObject.transform.position,
            Quaternion.identity,
            other.gameObject.transform);
    }
}

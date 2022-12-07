using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionLevelUpWorker : MonoBehaviour, LevelUpWorker
{
    [SerializeField] GameObject prefab;

    bool isEnabled = false;

    public void LevelUp(int newLevel)
    {
        isEnabled = true;
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (!isEnabled)
        {
            return;
        }

        Instantiate(
            prefab,
            bullet.transform.position,
            Quaternion.identity,
            this.transform);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float timer = 0f;

    [SerializeField] protected GameObject prefab;

    [SerializeField] protected float interval = 1f;

    [SerializeField] protected UpdatePositionBase updatePositionBase;

    public void Spawn()
    {
        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
        {
            return;
        }

        timer = interval;
        GameObject spawnedObject = Instantiate(prefab);
        updatePositionBase.UpdatePosition(spawnedObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    [SerializeField] Vector2RuntimeSet spawnPositions;

    private void OnEnable()
    {
        spawnPositions.Add(this.transform.position);
    }

    private void OnDisable()
    {
        spawnPositions.Remove(this.transform.position);
    }
}

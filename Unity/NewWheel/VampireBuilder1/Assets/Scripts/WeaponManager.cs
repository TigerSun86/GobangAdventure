using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;

    private void FixedUpdate()
    {
        foreach (GameObject prefab in prefabList)
        {
            Spawner spawner = prefab.GetComponent<Spawner>();
            Debug.Assert(spawner != null);
            spawner.Spawn();
        }
    }
}

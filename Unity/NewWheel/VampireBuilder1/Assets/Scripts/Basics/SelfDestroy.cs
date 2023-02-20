using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] float timeToLiveIfNeeded = 1f;

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public void DestroyGradually()
    {
        Destroy(gameObject, timeToLiveIfNeeded);
    }
}

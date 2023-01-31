using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBound : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            SelfDestroy selfDestroy = other.gameObject.GetComponent<SelfDestroy>();
            if (selfDestroy != null)
            {
                selfDestroy.Destroy();
            }
        }
    }
}

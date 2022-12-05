using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamagableUtilities
{
    public static bool IsDamagable(GameObject gameObject, string tag)
    {
        return gameObject.GetComponent<Damagable>() != null && gameObject.tag == tag;
    }

    public static bool IsDamagableEnemy(GameObject gameObject)
    {
        return IsDamagable(gameObject, "Enemy");
    }
}

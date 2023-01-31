using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningEffectManager : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    public void CreateEffect(List<Vector3> positions)
    {
        GameObject effectInstance = Instantiate(prefab);
        effectInstance.transform.SetParent(this.transform);
        LightningEffect lightningEffect = effectInstance.GetComponent<LightningEffect>();
        lightningEffect.SetPositions(positions[0], positions[1]);
    }
}

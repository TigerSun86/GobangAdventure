using UnityEngine;

[RequireComponent(typeof(DamageDataStorage))]
public class HealingEffect : MonoBehaviour
{
    private DamageDataStorage damageDataStorage;

    private void Awake()
    {
        damageDataStorage = GetComponent<DamageDataStorage>();
    }

    private void Update()
    {
        transform.position = damageDataStorage.damageData.gameObject.transform.position;
    }
}

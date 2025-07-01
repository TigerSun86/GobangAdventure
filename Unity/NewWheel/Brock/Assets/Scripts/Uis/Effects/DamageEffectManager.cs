using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageEffectManager : MonoBehaviour
{
    [SerializeField] List<DamageTypeAndEffect> effectPrefabs;

    public void CreateEffect(DamageData damageData)
    {
        if (!ShouldDisplay(damageData))
        {
            return;
        }

        List<GameObject> effects = effectPrefabs
            .Where(o => (o.damageType & damageData.damageType) > 0)
            .Select(o => o.effectPrefab)
            .ToList();
        if (!effects.Any())
        {
            Debug.LogWarning($"No effects found for damage type: {damageData.damageType}");
            return;
        }

        foreach (GameObject effect in effects)
        {
            GameObject effectInstance = Instantiate(effect);
            effectInstance.transform.SetParent(this.transform);
            effectInstance.GetComponent<DamageDataStorage>().damageData = damageData;
        }
    }

    private bool ShouldDisplay(DamageData damageData)
    {
        if (damageData.damageType == DamageType.HEALING && damageData.actualAmount == 0)
        {
            return false;
        }

        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    [SerializeField] GameObject damageUIPrefab;

    public void CreateDamageUI(DamageData damageData)
    {
        if (DamageUI.ShouldDisplay(damageData))
        {
            GameObject damageUI = Instantiate(damageUIPrefab);
            damageUI.GetComponent<DamageUI>().SetDamageData(damageData);
        }
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

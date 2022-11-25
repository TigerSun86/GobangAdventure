using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUIManager : MonoBehaviour
{
    [SerializeField] GameObject damageUIPrefab;

    void CreateDamageUI(int damage)
    {
        GameObject damageUI = Instantiate(damageUIPrefab);
        damageUI.GetComponent<DamageUI>().SetText(damage);
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

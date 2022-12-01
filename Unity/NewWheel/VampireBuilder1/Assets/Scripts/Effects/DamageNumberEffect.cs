using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Timers;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(DamageDataStorage))]
public class DamageNumberEffect : MonoBehaviour
{
    private TextMeshPro text;

    private DamageDataStorage damageDataStorage;

    private Vector3 moveVector;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        damageDataStorage = GetComponent<DamageDataStorage>();
        float moveSpeed = 1f;
        moveVector = Vector3.up * moveSpeed;
    }

    private void Start()
    {
        SetDamageData(damageDataStorage.damageData);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;

        text.alpha -= Time.deltaTime;
        if (text.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetDamageData(DamageData damageData)
    {
        transform.position = damageData.gameObject.transform.position;
        text.text = damageData.actualAmount.ToString();
        Color textColor;
        switch (damageData.damageType)
        {
            case DamageType.HEALING:
                textColor = Color.cyan;
                break;
            case DamageType.CRITICAL_HIT:
                textColor = Color.red;
                break;
            case DamageType.NORMAL_ATTACK:
            default:
                textColor = Color.yellow;
                break;
        }

        text.color = textColor;
    }
}

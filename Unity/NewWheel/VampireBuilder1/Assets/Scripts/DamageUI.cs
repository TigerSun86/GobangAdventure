using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Timers;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamageUI : MonoBehaviour
{
    TextMeshPro text;

    Vector3 moveVector;

    public static bool ShouldDisplay(DamageData damageData)
    {
        if (damageData.damageType == DamageType.HEALING && damageData.actualAmount == 0)
        {
            return false;
        }

        return true;
    }

    public void SetDamageData(DamageData damageData)
    {
        transform.position = damageData.gameObject.transform.position;
        text.text = damageData.actualAmount.ToString();
        Color textColor;
        switch (damageData.damageType)
        {
            case DamageType.HEALING:
                textColor = Color.cyan;
                break;
            case DamageType.NORMAL_ATTACK:
            default:
                textColor = Color.yellow;
                break;
        }

        text.color = textColor;
    }

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        float moveSpeed = 1f;
        moveVector = Vector3.up * moveSpeed;
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
}

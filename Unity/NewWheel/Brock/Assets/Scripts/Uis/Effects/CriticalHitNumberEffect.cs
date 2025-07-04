using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(DamageDataStorage))]
public class CriticalHitNumberEffect : MonoBehaviour
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
        if ((damageData.damageType & DamageType.CRITICAL_HIT) == 0)
        {
            return;
        }
        Vector3 randomPositionOffset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1.5f),
            0f
        );

        transform.position = damageData.gameObject.transform.position + randomPositionOffset;
        text.text = "CRIT!";
        text.color = Color.red;
    }
}

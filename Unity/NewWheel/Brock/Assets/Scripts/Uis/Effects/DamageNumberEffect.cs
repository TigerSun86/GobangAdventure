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
        Vector3 randomPositionOffset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1.5f),
            0f
        );

        transform.position = damageData.gameObject.transform.position + randomPositionOffset;
        text.text = damageData.actualAmount.ToString();
        Color textColor = Color.white; // Default color

        if ((damageData.damageType & DamageType.HEALING) != 0)
        {
            textColor = Color.cyan;
        }
        if ((damageData.damageType & DamageType.WEAK_ATTACK) != 0)
        {
            // Dark gray.
            textColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        }
        if ((damageData.damageType & DamageType.STRONG_ATTACK) != 0)
        {
            textColor = Color.red;
        }

        text.color = textColor;
    }
}

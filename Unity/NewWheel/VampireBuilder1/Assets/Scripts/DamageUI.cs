using System.Collections;
using System.Collections.Generic;
using Timers;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class DamageUI : MonoBehaviour
{
    TextMeshPro text;

    Vector3 moveVector;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        float moveSpeed = 10f;
        moveVector = Vector3.right * moveSpeed;
    }

    public void SetText(int damage)
    {
        text.text = damage.ToString();
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

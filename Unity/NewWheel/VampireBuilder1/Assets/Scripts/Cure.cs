using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : MonoBehaviour
{
    [SerializeField] float amount;

    [SerializeField] float interval;

    [SerializeField] Vector2 area;

    [SerializeField] string targetTag;

    float timer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            PerformCure();
            timer = interval;
        }
    }

    private void PerformCure()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(gameObject.transform.position, area, 0);
        foreach (Collider2D collider in colliders)
        {
            Health health = collider.gameObject.GetComponent<Health>();
            if (health != null && collider.gameObject.tag == targetTag)
            {
                health.IncreaseHealth((int)amount);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, area);
    }
}

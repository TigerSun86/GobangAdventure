using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
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
            Perform();
            timer = interval;
        }
    }

    private void Perform()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(gameObject.transform.position, area, 0);
        foreach (Collider2D collider in colliders)
        {
            Healable healable = collider.gameObject.GetComponent<Healable>();
            if (healable != null && collider.gameObject.tag == targetTag)
            {
                healable.TakeHealing((int)amount);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(gameObject.transform.position, area);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    [SerializeField] private String targetTag;
    [SerializeField] private UnityEvent attack;

    private bool _canAttack = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DealDamage(collision);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        DealDamage(collision);
    }

    private void DealDamage(Collider2D collision)
    {
        if (!_canAttack)
        {
            return;
        }

        if (collision.CompareTag(targetTag))
        {
            var damageable = collision.GetComponent<Damageable>();
            damageable.TakeDamage(1);
            TimersManager.SetTimer(this, 1, CanAttack);
            _canAttack = false;
            attack.Invoke();
        }
    }
    private void CanAttack()
    {
        _canAttack = true;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public string TargetTag { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TargetTag))
        {
            var damagable = collision.GetComponent<Damagable>();
            damagable.TakeDamage(1);
        }
    }
}

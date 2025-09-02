using System;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public event Action OnPickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tags.Player || collision.tag == Tags.PlayerWeapon)
        {
            OnPickup?.Invoke();
            Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    private float timer = 0f;

    [SerializeField] FloatVariable attackInterval;

    [SerializeField] FloatVariable attackArea;

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
        {
            return;
        }

        timer = attackInterval.value;

        GameObject gameObject = Instantiate(bullet);
        Transform playerTransform = Manager.instance.PlayerTransform;
        Vector3 position = new Vector3();
        position.x = playerTransform.position.x + (playerTransform.localScale.x / 2) + (gameObject.transform.localScale.x / 2);
        position.y = playerTransform.position.y;
        gameObject.transform.position = position;

        gameObject.transform.localScale *= attackArea.value;
    }
}

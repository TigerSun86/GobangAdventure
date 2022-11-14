using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    private void Awake()
    {
        gameObject.GetComponent<Attack>().TargetTag = "Enemy";
        SetPosition();
        TimersManager.SetTimer(this, 0.5f, () => Destroy(gameObject));
    }
    private void FixedUpdate()
    {
        SetPosition();
    }

    private void SetPosition()
    {
        Transform playerTransform = Manager.instance.PlayerTransform;
        Vector3 position = new Vector3();
        position.x = playerTransform.position.x + (playerTransform.localScale.x / 2) + (gameObject.transform.localScale.x / 2);
        position.y = playerTransform.position.y;
        gameObject.transform.position = position;
    }
}

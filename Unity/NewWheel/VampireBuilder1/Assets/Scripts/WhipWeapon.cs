using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class WhipWeapon : MonoBehaviour
{
    GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        Vector3 position = new Vector3();
        position.x = player.transform.position.x + (player.transform.localScale.x / 2) + (gameObject.transform.localScale.x / 2);
        position.y = player.transform.position.y;
        gameObject.transform.position = position;
    }
}

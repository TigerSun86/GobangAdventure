using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] GameObject whip;
    [SerializeField] float interval = 1f;
    float timer;

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer < 0f)
        {
            Instantiate(whip);

            timer = interval;
        }
    }
}

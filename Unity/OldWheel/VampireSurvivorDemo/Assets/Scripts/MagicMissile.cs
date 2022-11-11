using System.Collections;
using System.Collections.Generic;
using Timers;
using UnityEngine;
using UnityEngine.Events;

public class MagicMissile : MonoBehaviour
{
    [SerializeField] private MissileCreator missileCreator;
    [SerializeField] private UnityEvent launchMissile;

    private void LaunchMissile() 
    {
        missileCreator.CreateMissile();
        launchMissile.Invoke();
    }

    private void Awake()
    {
        TimersManager.SetLoopableTimer(this, 1, LaunchMissile);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private UnityEvent<int> healthChanged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Value 
    {
        get { return health; }
    }

    public void DecreaseHealth(int amount) 
    {
        health -= amount;
        healthChanged.Invoke(health);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Death : MonoBehaviour
{
    [SerializeField] private UnityEvent died;

    public void CheckDeath(int health)
    {
        if (health <= 0) 
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
        died.Invoke();
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

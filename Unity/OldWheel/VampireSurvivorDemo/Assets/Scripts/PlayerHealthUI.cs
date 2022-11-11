using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Health health;

    public void UpdateUI() 
    {
        healthBar.value = health.Value;
    }

    private void Awake()
    {
        healthBar.maxValue = health.Value;
        healthBar.value = health.Value;
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

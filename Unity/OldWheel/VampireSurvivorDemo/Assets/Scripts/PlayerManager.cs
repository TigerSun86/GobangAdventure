using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    
    private static PlayerManager _instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector2 Position 
    {
        get { return _instance.playerTransform.position; }
    }

    private void Awake()
    {
        _instance = this;
    }
}

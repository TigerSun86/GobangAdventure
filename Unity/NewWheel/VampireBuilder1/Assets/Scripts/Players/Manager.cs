using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    [SerializeField] GameObject player;

    public Transform PlayerTransform { get => player.GetComponent<Transform>(); }

    public Level PlayerLevel { get => player.GetComponent<Level>(); }

    public string PlayerTag { get => player.tag; }

    private void Awake()
    {
        instance = this;
    }
}

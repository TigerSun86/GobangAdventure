using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkySunManager : MonoBehaviour
{
    private GameObject prefabSun;

    private float createSunPosY = 6f;

    private float createSunMaxPosX = 4f;

    private float createSunMinPosX = -7f;

    private float sunDownMaxPosY = 3f;

    private float sunDownMinPosY = -4f;

    // Start is called before the first frame update
    void Start()
    {
        this.prefabSun = Resources.Load<GameObject>("Sun");
        InvokeRepeating("CreateSun", 0f, 3f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateSun()
    {
        Sun sun = GameObject.Instantiate<GameObject>(this.prefabSun, Vector3.zero, Quaternion.identity, this.transform).GetComponent<Sun>();
        float downTargetPosY = Random.Range(sunDownMinPosY, sunDownMaxPosY);
        float createPosX = Random.Range(createSunMinPosX, createSunMaxPosX);
        float createPosY = createSunPosY;
        sun.InitForSky(downTargetPosY, createPosX, createPosY);
    }
}

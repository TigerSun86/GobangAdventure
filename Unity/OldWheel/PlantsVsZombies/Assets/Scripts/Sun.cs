using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    private float downTargetPosY;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= this.downTargetPosY)
        {
            Invoke("DestroySun", 5);
            return;
        }

        transform.Translate(Vector3.down * Time.deltaTime);
    }

    public void InitForSky(float downTargetPosY, float createPosX, float createPosY)
    {
        this.downTargetPosY = downTargetPosY;
        transform.position = new Vector2(createPosX, createPosY);
    }

    private void DestroySun()
    {
        Destroy(gameObject);
    }
}

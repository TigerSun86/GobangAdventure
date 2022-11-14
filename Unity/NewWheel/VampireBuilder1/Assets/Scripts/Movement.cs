using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] protected UpdatePositionBase updatePositionBase;

    private void FixedUpdate()
    {
        updatePositionBase.UpdatePosition(gameObject);
    }
}

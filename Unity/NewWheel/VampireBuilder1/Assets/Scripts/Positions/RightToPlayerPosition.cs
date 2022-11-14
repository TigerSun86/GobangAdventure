using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightToPlayerPosition : UpdatePositionBase
{
    public override void UpdatePosition(GameObject gameObject)
    {
        Transform playerTransform = Manager.instance.PlayerTransform;
        Vector3 position = new Vector3();
        position.x = playerTransform.position.x + (playerTransform.localScale.x / 2) + (gameObject.transform.localScale.x / 2);
        position.y = playerTransform.position.y;
        gameObject.transform.position = position;
    }
}

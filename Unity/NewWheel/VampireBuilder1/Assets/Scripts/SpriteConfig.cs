using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteConfig : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] private Color idleColor = Color.white;
    [SerializeField] private Color damagedColor = Color.red;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetIdleColor();
    }

    public void SetIdleColor()
    {
        spriteRenderer.color = idleColor;
    }

    public void SetDamagedColor() 
    {
        spriteRenderer.color = damagedColor;
    }
}

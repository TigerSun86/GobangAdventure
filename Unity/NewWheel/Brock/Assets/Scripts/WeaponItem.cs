using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WeaponItem : MonoBehaviour
{
    public int isMovingCount;

    private WeaponSuit weaponSuit;

    public void Initialize(WeaponSuit weaponSuit)
    {
        this.weaponSuit = weaponSuit;
    }

    public void MoveToTarget(Transform target, float remainingTime)
    {
        Move(target, remainingTime);
    }

    public void ReturnToStand(float remainingTime)
    {
        Move(this.weaponSuit.transform, remainingTime);
    }

    public void SetVisibility(bool visible)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = visible;
    }

    private void Start()
    {
        ChangeSprite();
    }

    private void FixedUpdate()
    {
        if (this.isMovingCount > 0)
        {
            this.isMovingCount--;
        }
        else
        {
            // Prevent the case when the parent is pushed away but this is staying here.
            this.transform.position = this.transform.parent.position;
        }
    }

    private void ChangeSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.weaponSuit.weaponConfig.sprite;
    }

    private void Move(Transform target, float remainingTime)
    {
        this.isMovingCount = 2;
        if (remainingTime > 0.01f && !target.IsDestroyed())
        {
            Vector3 direction = target.position - this.transform.position;
            float dynamicSpeed = direction.magnitude / remainingTime;
            direction.Normalize();
            this.transform.position += dynamicSpeed * direction * Time.fixedDeltaTime;
        }
    }
}

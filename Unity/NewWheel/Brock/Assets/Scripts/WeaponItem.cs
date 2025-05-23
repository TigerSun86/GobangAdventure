using Unity.VisualScripting;
using UnityEngine;

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
        switch (this.weaponSuit.weaponBaseType)
        {
            case WeaponBaseType.ROCK:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Circle");
                break;
            case WeaponBaseType.PAPER:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Square");
                break;
            case WeaponBaseType.SCISSOR:
                spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Triangle");
                break;
            default:
                Debug.LogError("Invalid weapon base type");
                break;
        }
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

using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponBaseType weaponBaseType;

    public SkillConfig[] skills;

    public void SetSkill(SkillConfig[] skills)
    {
        GetComponent<SkillActor>().Initialize(skills);
    }

    private void Start()
    {
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        switch (this.weaponBaseType)
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
}

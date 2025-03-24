using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponBaseType weaponBaseType;

    Rigidbody2D rb;

    [SerializeField] public float range;

    public Transform target;

    public AttackActionStage attackActionStage;

    public float timeInCurrentStage;

    public int pierceCount;

    public ShopItem shopItem;

    public SkillActor skillActor;

    public SkillConfig[] skills;

    private Dictionary<AttackActionStage, float> attackActionStageDuration = new Dictionary<AttackActionStage, float>
    {
        { AttackActionStage.ANTICIPATION, 0.2f },
        { AttackActionStage.RECOVERY, 0.2f },
        { AttackActionStage.COOL_DOWN, 2f }
    };


    private Dictionary<AttackActionStage, float> attackEffectiveTime = new Dictionary<AttackActionStage, float>
    {
        { AttackActionStage.ANTICIPATION, 0.05f },
    };

    public void SetSkill(SkillConfig[] skills)
    {
        GetComponent<SkillActor>().Initialize(skills);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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

    private void FixedUpdate()
    {
        // PerformAttackAction();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // DealDamage(other);
    }

    private void DealDamage(Collider2D other)
    {
        if (!IsAttackEffective() || other.gameObject.IsDestroyed() || other.gameObject.GetComponent<DefenceArea>() == null)
        {
            return;
        }

        if (GetTargetTag() == other.tag)
        {
            this.pierceCount--;
            Damagable damagable = other.gameObject.GetComponent<Damagable>();

            double damage = 2;
            if (shopItem != null)
            {
                damage = shopItem.attack;
            }

            DamageType damageType = DamageType.NORMAL_ATTACK;
            WeaponBaseTypeMatchResult matchResult = WeaponBaseTypeUtility.GetMatchResult(this.weaponBaseType, other.gameObject.GetComponent<DefenceArea>().weaponBaseType);
            if (matchResult == WeaponBaseTypeMatchResult.STRONG)
            {
                damageType = DamageType.CRITICAL_HIT;
                damage *= 2;
            }
            else if (matchResult == WeaponBaseTypeMatchResult.WEAK)
            {
                damageType = DamageType.WEAK_ATTACK;
                damage /= 2;
            }
            damagable.TakeDamage((int)damage, damageType);
        }
    }

    private void PerformAttackAction()
    {
        this.timeInCurrentStage += Time.fixedDeltaTime;
        switch (this.attackActionStage)
        {
            case AttackActionStage.STAND_BY:
                HandleStandBy();
                break;
            case AttackActionStage.ANTICIPATION:
                HandleAnticipation();
                break;
            case AttackActionStage.RECOVERY:
                HandleRecovery();
                break;
            case AttackActionStage.COOL_DOWN:
                HandleCoolDown();
                break;
            default:
                Debug.LogError("Invalid attack action stage");
                break;
        }
    }

    private void HandleStandBy()
    {
        transform.position = transform.parent.position;
        FindClosestTarget();
        if (target != null)
        {
            this.pierceCount = 1;
            SwitchAttackActionStage(AttackActionStage.ANTICIPATION);
        }
    }

    private void HandleAnticipation()
    {
        float remainingTime = attackActionStageDuration[AttackActionStage.ANTICIPATION] - timeInCurrentStage;
        Move(transform, target, remainingTime);
        if (attackActionStageDuration[AttackActionStage.ANTICIPATION] <= timeInCurrentStage)
        {
            SwitchAttackActionStage(AttackActionStage.RECOVERY);
        }
    }

    private void HandleRecovery()
    {
        float remainingTime = attackActionStageDuration[AttackActionStage.RECOVERY] - timeInCurrentStage;
        Move(transform, transform.parent, remainingTime);
        if (attackActionStageDuration[AttackActionStage.RECOVERY] <= timeInCurrentStage)
        {
            SwitchAttackActionStage(AttackActionStage.COOL_DOWN);
        }
    }

    private void HandleCoolDown()
    {
        transform.position = transform.parent.position;
        if (attackActionStageDuration[AttackActionStage.COOL_DOWN] <= timeInCurrentStage)
        {
            SwitchAttackActionStage(AttackActionStage.STAND_BY);
            this.target = null;
        }
    }

    private void SwitchAttackActionStage(AttackActionStage attackActionStage)
    {
        this.attackActionStage = attackActionStage;
        this.timeInCurrentStage = 0;
    }

    private String GetParentTag()
    {
        return transform.parent.tag;
    }

    private String GetTargetTag()
    {
        String tag = GetParentTag();
        if (tag == "Player")
        {
            return "Enemy";
        }
        else if (tag == "Enemy")
        {
            return "Player";
        }
        else
        {
            Debug.LogError("Invalid tag");
            return null;
        }
    }

    private void FindClosestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(GetTargetTag());
        float minDistance = float.MaxValue;
        Transform closestTarget = null;
        foreach (GameObject targetTemp in targets)
        {
            if (targetTemp.GetComponent<DefenceArea>() == null)
            {
                continue;
            }
            float distance = Vector3.Distance(targetTemp.transform.position, transform.parent.position);
            if (distance <= this.range && distance < minDistance)
            {
                minDistance = distance;
                closestTarget = targetTemp.transform;
            }
        }

        if (closestTarget != null)
        {
            this.target = closestTarget;
        }
    }

    private void Move(Transform source, Transform target, float remainingTime)
    {
        if (remainingTime > 0.01f && !source.IsDestroyed() && !target.IsDestroyed())
        {
            Vector3 direction = target.position - source.position;
            float dynamicSpeed = direction.magnitude / remainingTime;
            direction.Normalize();
            rb.linearVelocity = dynamicSpeed * direction;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool IsAttackEffective()
    {
        return this.attackEffectiveTime.ContainsKey(this.attackActionStage) && this.timeInCurrentStage >= this.attackEffectiveTime[this.attackActionStage] && this.pierceCount > 0;
    }
}

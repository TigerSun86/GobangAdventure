using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TianShanLiuYangZhang : SkillStage2
{
    private static readonly int X_MAX = 99;

    private static readonly int Y_MIN = -5;

    private static readonly float STAGE2_ATTACK_FACTOR = 0.25f;

    [SerializeField] SkillStage skillStage = SkillStage.STAGE1;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    bool isFirstAttack = true;

    private Vector2 returnBackPosition;

    public override SkillId GetSkillId()
    {
        return SkillId.TIAN_SHAN_LIU_YANG_ZHANG;
    }

    public void Attack(Collider2D other, GameObject bullet)
    {
        if (isFirstAttack)
        {
            isFirstAttack = false;
            returnBackPosition = transform.position;
        }

        float? attackOverride = null;
        if (skillStage == SkillStage.STAGE2)
        {
            attackOverride = skillAttributeManager.GetAttribute(this.GetSkillId(), AttributeType.ATTACK) * STAGE2_ATTACK_FACTOR;
        }

        AttackData attackBase = new AttackData(GetSkillId(), skillAttributeManager, attackOverride);
        attackTargetSelectEvent.Invoke(other.gameObject, attackBase);

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, this.gameObject.transform.position, Quaternion.identity, this.transform);
        }
    }

    public void EnterStage2()
    {
        skillStage = SkillStage.STAGE2;
        SetPierceCount((int)skillAttributeManager.GetAttribute(this.GetSkillId(), AttributeType.PIERCE));

        SetStage2MovePath();
    }

    private void Start()
    {
        SetPierceCount(999);

        returnBackPosition = transform.position;
        skillStage = SkillStage.STAGE1;
        SetStage1MovePath();
    }

    private void SetStage1MovePath()
    {
        Vector2 startPoint = transform.position;
        Vector2 endPoint = transform.position + new Vector3(X_MAX, 0);
        List<Vector2> path = new List<Vector2>() { startPoint, endPoint };
        MoveAlongPath move = GetComponent<MoveAlongPath>();
        move.SetPath(path);
    }

    private void SetStage2MovePath()
    {
        Vector2 startPoint = transform.position;
        Vector2 endPoint = returnBackPosition;
        Vector2 midPoint = new Vector2((startPoint.x + endPoint.x) / 2, Y_MIN);
        List<Vector2> path = new List<Vector2>() { startPoint, midPoint, endPoint };
        MoveAlongPath move = GetComponent<MoveAlongPath>();
        move.SetPath(path, isCurve: true);
    }

    private void SetPierceCount(int count)
    {
        HitTarget hitTarget = GetComponents<HitTarget>().Where(h => h.targetTag == "Enemy").FirstOrDefault();
        if (hitTarget != null)
        {
            hitTarget.maxHitCount = count;
        }
    }
}

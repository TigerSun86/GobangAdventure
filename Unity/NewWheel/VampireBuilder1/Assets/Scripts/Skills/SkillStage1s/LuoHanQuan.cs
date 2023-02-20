using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LuoHanQuan : SkillStage2
{
    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    float timer;

    public override SkillId GetSkillId()
    {
        return SkillId.LUO_HAN_QUAN;
    }

    public void Attack(Collider2D other, GameObject bullet)
    {
        AttackData attackBase = new AttackData(GetSkillId(), skillAttributeManager);
        attackTargetSelectEvent.Invoke(other.gameObject, attackBase);

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, this.gameObject.transform.position, Quaternion.identity, this.transform);
        }
    }

    private void Start()
    {
        HitTarget hitTarget = GetComponents<HitTarget>().Where(h => h.targetTag == "Enemy").FirstOrDefault();
        if (hitTarget != null)
        {
            hitTarget.maxHitCount = (int)skillAttributeManager.GetAttribute(this.GetSkillId(), AttributeType.PIERCE);
        }
    }
}

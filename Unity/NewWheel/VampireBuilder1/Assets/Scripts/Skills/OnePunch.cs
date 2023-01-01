using UnityEngine;
using UnityEngine.Events;

public class OnePunch : SkillPrefab
{
    [SerializeField] MainSkill mainSkill;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    private void Start()
    {
        AttackData attackBase = new AttackData(mainSkill);
        attackTargetSelectEvent.Invoke(target, attackBase);

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, this.gameObject.transform.position, Quaternion.identity, this.transform);
        }
    }
}
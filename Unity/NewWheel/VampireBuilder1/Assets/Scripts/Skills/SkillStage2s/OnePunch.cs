using UnityEngine;
using UnityEngine.Events;

public class OnePunch : SkillStage2
{
    // Leave child effect some time to display
    private static readonly float TIME_TO_LIVE = 2f;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    float timer;

    public override SkillId GetSkillId()
    {
        return SkillId.ONE_PUNCH;
    }

    private void Start()
    {
        timer = TIME_TO_LIVE;
        AttackData attackBase = new AttackData(GetSkillId(), skillAttributeManager);
        attackTargetSelectEvent.Invoke(target, attackBase);

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, this.gameObject.transform.position, Quaternion.identity, this.transform);
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

public class OnePunch : SkillPrefab
{
    // Leave child effect some time to display
    private static readonly float TIME_TO_LIVE = 2f;

    [SerializeField] MainSkill mainSkill;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    float timer;

    private void Start()
    {
        timer = TIME_TO_LIVE;
        AttackData attackBase = new AttackData(mainSkill);
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
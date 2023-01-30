using UnityEngine;
using UnityEngine.Events;

public class Explosion : SkillPrefab
{
    // Used to display Gizmos
    private static readonly float TIME_TO_LIVE = 1f;

    [SerializeField] MainSkill mainSkill;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    float radius;

    float timer;

    public override SkillId GetSkillId()
    {
        return SkillId.EXPLOSION;
    }

    private void Start()
    {
        timer = TIME_TO_LIVE;
        AttackData attackBase = new AttackData(mainSkill);
        radius = mainSkill.area;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.gameObject.transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            if (DamagableUtilities.IsDamagableEnemy(collider.gameObject))
            {
                attackTargetSelectEvent.Invoke(collider.gameObject, attackBase);
            }
        }

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, this.gameObject.transform.position, Quaternion.identity, this.transform);
            effectInstance.transform.localScale *= radius;
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.gameObject.transform.position, radius);
    }
}
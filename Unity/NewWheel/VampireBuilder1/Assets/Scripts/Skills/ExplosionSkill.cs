using UnityEngine;
using UnityEngine.Events;

public class ExplosionSkill : SkillBase
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] FloatVariable attackAreaFactor;

    [SerializeField] CriticalHit criticalHit;

    [SerializeField] GameObject effect;

    [SerializeField] UnityEvent<GameObject, AttackData> attackTargetSelectEvent;

    [SerializeField] AttackData attackBase = new AttackData(0.5f);

    float radius;

    private Vector2? debugPosition;

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override string GetName()
    {
        return "Explosion";
    }

    public override string GetNextLevelDescription()
    {
        return "Explode after hit an enemy";
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (GetLevel() == 0)
        {
            return;
        }

        float attack = (int)(attackFactor.value / 2);
        debugPosition = bullet.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bullet.transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            if (DamagableUtilities.IsDamagableEnemy(collider.gameObject))
            {
                attackTargetSelectEvent.Invoke(collider.gameObject, attackBase);
            }
        }

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, bullet.transform.position, Quaternion.identity, this.transform);
            effectInstance.transform.localScale *= attackAreaFactor.value * 2;
        }
    }

    private void Start()
    {
        radius = attackAreaFactor.value + 1;
    }

    void OnDrawGizmos()
    {
        if (debugPosition.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugPosition.Value, radius);
        }
    }
}
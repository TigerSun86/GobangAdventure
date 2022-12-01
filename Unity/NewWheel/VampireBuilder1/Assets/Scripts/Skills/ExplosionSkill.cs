using UnityEngine;

public class ExplosionSkill : SkillBase
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] FloatVariable attackAreaFactor;

    [SerializeField] CriticalHit criticalHit;

    [SerializeField] GameObject effect;

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

        float radius = attackAreaFactor.value * 2;
        float attack = (int)(attackFactor.value / 2);
        debugPosition = bullet.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(bullet.transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            Damagable damagable = collider.gameObject.GetComponent<Damagable>();
            if (damagable != null && collider.gameObject.tag == "Enemy")
            {
                float damage = attack;
                DamageType damageType = DamageType.NORMAL_ATTACK;
                if (criticalHit != null)
                {
                    (damage, damageType) = criticalHit.CalculateDamage(damage);
                }

                damagable.TakeDamage((int)damage, damageType);
            }
        }

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect, bullet.transform.position, Quaternion.identity, this.transform);
            effectInstance.transform.localScale *= attackAreaFactor.value * 2;
        }
    }

    void OnDrawGizmos()
    {
        if (debugPosition.HasValue)
        {
            float radius = attackAreaFactor.value * 2;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(debugPosition.Value, radius);
        }
    }
}
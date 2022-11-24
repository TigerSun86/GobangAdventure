using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

public class ExplosionSkill : SkillBase
{
    [SerializeField] FloatVariable attackFactor;

    [SerializeField] FloatVariable attackAreaFactor;

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

        float radius = attackAreaFactor.value * 5;
        int attack = (int)(attackFactor.value / 2);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(bullet.transform.position, radius);
        foreach (Collider2D collider in colliders)
        {
            Damagable damagable = collider.gameObject.GetComponent<Damagable>();
            if (damagable != null && collider.gameObject.tag == "Enemy")
            {
                damagable.TakeDamage(attack);
            }
        }
    }
}

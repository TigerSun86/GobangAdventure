using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChainLightning : MonoBehaviour
{
    private static readonly float ATTACK_INTERVAL = 0.2f;

    [SerializeField] IntVariable maxCount;

    [SerializeField] FloatVariable attackDecreaseRate;

    [SerializeField] FloatVariable initialAttack;

    [SerializeField] FloatVariable attackFactor;

    [SerializeField] FloatVariable attackAreaFactor;

    [SerializeField] CriticalHit criticalHit;

    [SerializeField] GameObject effect;

    int remainingCount;

    float currentAttack;

    float radius;

    private Vector2? debugPosition;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        remainingCount = maxCount.value;
        currentAttack = initialAttack.value;
        radius = attackAreaFactor.value + 2;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
        {
            return;
        }

        timer = ATTACK_INTERVAL;

        if (remainingCount <= 0)
        {
            Destroy(this.gameObject);
            return;
        }

        remainingCount--;

        float attack = currentAttack;
        currentAttack *= (1 - attackDecreaseRate.value);
        debugPosition = this.transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, radius);
        Collider2D target = PositionUtilities.FindRandom(
            colliders,
            (o) => o.gameObject.tag == "Enemy" && o.gameObject.GetComponent<Damagable>() != null,
            (o) => o.gameObject.transform.position);
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (effect != null)
        {
            GameObject effectInstance = Instantiate(effect);
            LightningEffect lightningEffect = effectInstance.GetComponent<LightningEffect>();
            lightningEffect.SetPositions(this.transform.position, target.gameObject.transform.position);
        }

        float damage = attack;
        DamageType damageType = DamageType.NORMAL_ATTACK;
        if (criticalHit != null)
        {
            (damage, damageType) = criticalHit.CalculateDamage(damage);
        }

        Damagable damagable = target.gameObject.GetComponent<Damagable>();
        damagable.TakeDamage((int)damage, damageType);
        this.transform.position = target.gameObject.transform.position;
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

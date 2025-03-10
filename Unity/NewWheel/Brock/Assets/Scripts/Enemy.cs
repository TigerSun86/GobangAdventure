using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed;

    public AiStrategy aiStrategy;

    Transform target;

    DefenceArea defenceArea;
    Health health;

    private void Awake()
    {

        target = GameObject.Find("Player").transform;
        defenceArea = GetComponent<DefenceArea>();
        defenceArea.SetCharacter(gameObject);
        health = GetComponent<Health>();
    }

    public void SetWeapon(GameObject weaponPrefab)
    {
        this.defenceArea.SetWeapon(weaponPrefab);
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            if (direction.magnitude <= GetShortestWeaponRange() - 0.2 && !aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth))
            {
                return;
            }
            if (aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth) && health.health < (health.maxHealth / 2f))
            {
                direction *= -1;
            }
            direction.Normalize();
            transform.position += direction * speed * Time.deltaTime;
        }
    }


    private float GetShortestWeaponRange()
    {
        return this.defenceArea.weapon.range;
    }
}

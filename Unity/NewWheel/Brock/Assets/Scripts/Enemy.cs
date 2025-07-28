using UnityEngine;

[RequireComponent(typeof(DieWithDependency))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField, Required]
    private GameObject weaponSuitPrefab;

    [SerializeField, AssignedInCode]
    private EnemyConfig enemyConfig;

    [SerializeField, AssignedInCode]
    private Vector3 targetPosition;

    private Vector3 defaultTargetPosition;

    private Transform playerWeaponTransform;

    private WeaponSuit weaponSuit;

    public void Initialize(EnemyConfig enemyConfig)
    {
        this.enemyConfig = enemyConfig;
        GameObject allyTower = GameObject.Find("AllyTower");
        if (allyTower == null)
        {
            Debug.LogError("AllyTower not found in the scene. Enemy cannot initialize without it.");
            return;
        }

        // The default target position should be set by DirectionTrigger. This setup is just for extra safety.
        this.defaultTargetPosition = allyTower.transform.position;

        GameObject weaponSuitObject = Instantiate(weaponSuitPrefab, transform.position, Quaternion.identity, transform);
        weaponSuitObject.tag = Tags.EnemyWeapon;
        this.weaponSuit = weaponSuitObject.GetComponent<WeaponSuit>();
        this.weaponSuit.Initialize(this.enemyConfig.weaponConfig);
        DieWithDependency death = GetComponent<DieWithDependency>();
        death.dependency = weaponSuitObject;
    }

    public void ChangeDefaultTargetPosition(Vector3 newDefaultTargetPosition)
    {
        this.defaultTargetPosition = newDefaultTargetPosition;
    }

    private void Start()
    {
        this.playerWeaponTransform = null;
    }

    private void FixedUpdate()
    {
        if (this.playerWeaponTransform != null && !this.playerWeaponTransform.gameObject.activeInHierarchy)
        {
            // Player weapon might be destroyed while within the enemy detecting range.
            this.playerWeaponTransform = null;
        }

        // If player weapon is detected, use it as target; otherwise, use ally tower position.
        if (this.playerWeaponTransform != null)
        {
            this.targetPosition = this.playerWeaponTransform.position;
        }
        else
        {
            this.targetPosition = this.defaultTargetPosition;
        }

        if (IsHealing())
        {
            // Stay.
            return;
        }

        if (this.enemyConfig.aiStrategy.HasFlag(AiStrategy.RunAwayWhenLowHealth)
            && this.weaponSuit.GetHealth().health < (this.weaponSuit.GetHealth().maxHealth / 2f)
            // When attacking ally tower don't run away because tower cannot chase.
            && this.targetPosition != this.defaultTargetPosition)
        {
            MoveAway();
            return;
        }

        if (IsTargetFarAway())
        {
            MoveToTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.PlayerWeapon))
        {
            this.playerWeaponTransform = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (this.playerWeaponTransform != null && collision.transform == this.playerWeaponTransform)
        {
            this.playerWeaponTransform = null;
        }
    }

    private bool IsHealing()
    {
        return this.weaponSuit.skillActor.IsHealing();
    }

    private void MoveAway()
    {
        if (!this.weaponSuit.capabilityController.Can(CapabilityType.Move))
        {
            return;
        }

        Vector3 direction = this.transform.position - this.targetPosition;
        direction.Normalize();
        this.transform.position += direction * speed * Time.deltaTime;
    }

    private void MoveToTarget()
    {
        if (!this.weaponSuit.capabilityController.Can(CapabilityType.Move))
        {
            return;
        }

        Vector3 direction = this.targetPosition - this.transform.position;
        direction.Normalize();
        this.transform.position += direction * speed * Time.deltaTime;
    }

    private bool IsTargetFarAway()
    {
        return Vector3.Distance(this.transform.position, this.targetPosition)
            > GetShortestWeaponRange();
    }

    private float GetShortestWeaponRange()
    {
        if (this.weaponSuit.skillActor.GetSkillAttack() != null)
        {
            return this.weaponSuit.skillActor.GetSkillAttack().skillConfig.range;
        }
        return 0;
    }
}

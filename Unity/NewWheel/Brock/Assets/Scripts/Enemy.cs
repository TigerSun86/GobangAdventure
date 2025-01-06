using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = .2f;

    Transform target;

    DefenceArea defenceArea;

    private void Awake()
    {

        target = GameObject.Find("Player").transform;
        defenceArea = GetComponent<DefenceArea>();
        defenceArea.SetCharacter(gameObject);
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
            if (direction.magnitude <= GetShortestWeaponRange() - 0.2)
            {
                return;
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

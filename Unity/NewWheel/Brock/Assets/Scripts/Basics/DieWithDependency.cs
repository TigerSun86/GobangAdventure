using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SelfDestroy))]
public class DieWithDependency : MonoBehaviour
{
    [SerializeField] public UnityEvent<GameObject> deathEvent;

    [SerializeField] public GameObject dependency;

    [SerializeField, AssignedInCode] SelfDestroy selfDestroy;

    private void Start()
    {
        Death dependencyDeath = dependency.GetComponent<Death>();
        dependencyDeath.deathEvent.AddListener(Die);

        selfDestroy = GetComponent<SelfDestroy>();
    }

    private void Die(GameObject dependencyObject)
    {
        deathEvent.Invoke(gameObject);
        selfDestroy.Destroy();
    }
}

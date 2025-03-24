using UnityEngine;

[CreateAssetMenu(menuName = "FleetConfig")]
public class FleetConfig : ScriptableObject
{
    [SerializeField] public EnemyInFleetConfig[] enemyInFleetConfig;
}
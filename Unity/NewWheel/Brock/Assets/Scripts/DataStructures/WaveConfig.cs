using UnityEngine;

[CreateAssetMenu(menuName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] public FleetConfig[] fleetConfigs;

    [SerializeField] public EnemyInFleetConfig[] enemyInFleetConfigs;
}
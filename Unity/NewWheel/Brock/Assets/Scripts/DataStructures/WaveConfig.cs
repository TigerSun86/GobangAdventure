using UnityEngine;

[CreateAssetMenu(menuName = "WaveConfig")]
public class WaveConfig : ScriptableObject
{
    [SerializeField] public FleetConfig[] fleetConfigs;
}
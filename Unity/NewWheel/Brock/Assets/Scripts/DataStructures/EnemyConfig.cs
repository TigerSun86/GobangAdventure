using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public WeaponConfig weaponConfig;

    public AiStrategy aiStrategy;
}
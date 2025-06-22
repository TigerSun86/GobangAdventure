using UnityEngine;

[CreateAssetMenu(menuName = "EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public WeaponConfig weaponConfig;

    public WeaponConfig2 weaponConfig2;

    public AiStrategy aiStrategy;
}
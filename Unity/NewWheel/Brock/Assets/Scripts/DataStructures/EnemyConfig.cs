using System;

[Serializable]
public class EnemyConfig
{
    public string enemyId;

    public WeaponConfig weaponConfig;

    public AiStrategy aiStrategy;

    public LootConfig lootConfig;
}
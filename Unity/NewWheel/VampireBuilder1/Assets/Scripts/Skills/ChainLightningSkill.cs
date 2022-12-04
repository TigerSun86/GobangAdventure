using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightningSkill : SkillBase
{
    [SerializeField] GameObject chainLightningPrefab;

    [SerializeField] IntVariable maxCount;

    [SerializeField] FloatVariable attackDecreaseRate;

    [SerializeField] int changeAmount = 1;

    public override void LevelUp()
    {
        base.LevelUp();
        maxCount.ApplyChange(changeAmount);
    }

    public override string GetName()
    {
        return "Chain Lightning";
    }

    public override string GetNextLevelDescription()
    {
        return $"Hurls a bolt of lightning that bounces up to {maxCount.value + changeAmount} times. Each jump deals {attackDecreaseRate.value} less damage";
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (GetLevel() == 0)
        {
            return;
        }

        GameObject chainLightningInstance = Instantiate(
            chainLightningPrefab,
            other.gameObject.transform.position,
            Quaternion.identity,
            this.transform);
    }
}

using UnityEngine;

public class ExplosionSkill : SkillBase
{
    [SerializeField] GameObject prefab;

    public override void LevelUp()
    {
        base.LevelUp();
    }

    public override string GetName()
    {
        return "Explosion";
    }

    public override string GetNextLevelDescription()
    {
        return "Explode after hit an enemy";
    }

    public void Run(Collider2D other, GameObject bullet)
    {
        if (GetLevel() == 0)
        {
            return;
        }

        Instantiate(
            prefab,
            bullet.transform.position,
            Quaternion.identity,
            this.transform);
    }
}
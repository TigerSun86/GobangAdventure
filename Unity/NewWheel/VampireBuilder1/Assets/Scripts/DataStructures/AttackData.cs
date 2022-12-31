using System;

[Serializable]
public class AttackData
{
    public float attack;

    public float criticalRate;

    public float criticalAmount;

    public AttackData(float attack, float criticalRate = 0, float criticalAmount = 2)
    {
        this.attack = attack;
        this.criticalRate = criticalRate;
        this.criticalAmount = criticalAmount;
    }

    public AttackData(MainSkill mainSkill)
    {
        this.attack = mainSkill.attack;
        this.criticalRate = mainSkill.criticalRate;
        this.criticalAmount = mainSkill.criticalAmount;
    }
}
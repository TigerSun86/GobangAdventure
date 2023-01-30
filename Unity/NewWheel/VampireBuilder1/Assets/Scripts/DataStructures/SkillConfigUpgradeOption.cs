public class SkillConfigUpgradeOption : UpgradeOption
{
    public SkillConfigUpgradeOption(SkillConfig skill, int level)
    {
        this.skillId = skill.id;
        this.upgradeName = skill.name;
        this.description = skill.GetLevelDescription(level);
        if (level == 1)
        {
            this.levelText = "New";
        }
        else
        {
            this.levelText = $"L{level}";
        }
    }

    public override void TriggerUpgrade()
    {
    }
}

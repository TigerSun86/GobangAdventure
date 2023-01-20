public class SkillConfigUpgradeOption : UpgradeOption
{
    public SkillConfigUpgradeOption(SkillConfig skill, int level)
    {
        this.skillId = skill.id;
        this.upgradeName = skill.name;

        if (level == 1)
        {
            this.levelText = "New";
            this.description = skill.description;
        }
        else
        {
            this.levelText = $"L{level}";
            this.description = skill.GetLevelDescription(level);
        }
    }

    public override void TriggerUpgrade()
    {
    }
}

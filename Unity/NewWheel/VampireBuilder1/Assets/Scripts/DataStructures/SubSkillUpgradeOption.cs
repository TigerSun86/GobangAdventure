public class SubSkillUpgradeOption : UpgradeOption
{
    public SubSkill subSkill;

    public SubSkillUpgradeOption(SubSkill subSkill)
    {
        this.subSkill = subSkill;
        SubSkillLevelInfo nextLevel = subSkill.GetNextLevelInfo();
        this.levelText = $"L{nextLevel.level.ToString()}";
        this.upgradeName = $"{subSkill.mainSkill.name} - {subSkill.name}";
        this.description = nextLevel.description;
    }

    public override void TriggerUpgrade()
    {
        this.subSkill.LevelUp();
    }
}

public class MainSkillUpgradeOption : UpgradeOption
{
    public MainSkill mainSkill;

    public MainSkillUpgradeOption(MainSkill mainSkill)
    {
        this.mainSkill = mainSkill;
        this.levelText = "New";
        this.upgradeName = mainSkill.name;
        this.description = mainSkill.description;
    }

    public override void TriggerUpgrade()
    {
        this.mainSkill.Enable();
    }
}

using TMPro;
using UnityEngine;

public class SkillUpgradeButton : MonoBehaviour
{
    [SerializeField] UpgradeOption skill;
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI skillDescriptionText;

    public void SetSkill(UpgradeOption skill)
    {
        this.skill = skill;
        skillNameText.text = skill.upgradeName;
        levelText.text = skill.levelText;
        skillDescriptionText.text = skill.description;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void Upgrade()
    {
        skill.TriggerUpgrade();
    }
}

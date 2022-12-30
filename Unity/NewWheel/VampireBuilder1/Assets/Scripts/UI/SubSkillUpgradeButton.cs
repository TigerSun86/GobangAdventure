using TMPro;
using UnityEngine;

public class SubSkillUpgradeButton : MonoBehaviour
{
    [SerializeField] SubSkill skill;

    TextMeshProUGUI buttonText;

    private void OnEnable()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void SetSkill(SubSkill skill)
    {
        this.skill = skill;
        RefreshText();
    }

    public void Upgrade()
    {
        skill.LevelUp();
        RefreshText();
    }

    private void RefreshText()
    {
        buttonText.text = $"L{skill.currentLevel} {skill.skillName}";
    }
}

using TMPro;
using UnityEngine;

public class SubSkillUI : MonoBehaviour
{
    [SerializeField] SubSkill skill;

    TextMeshProUGUI text;

    private void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetSkill(SubSkill skill)
    {
        this.skill = skill;
        RefreshText();
    }

    public void Initialize()
    {
        skill = null;
        text.text = string.Empty;
    }

    private void RefreshText()
    {
        text.text = $"L{skill.currentLevel} {skill.skillName}";
    }
}

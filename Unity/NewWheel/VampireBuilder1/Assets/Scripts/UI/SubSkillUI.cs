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

    private void RefreshText()
    {
        if (skill != null)
        {
            text.text = $"L{skill.currentLevel} {skill.skillName}";
        }
        else
        {
            text.text = string.Empty;
        }
    }
}

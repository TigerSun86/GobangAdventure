using TMPro;
using UnityEngine;

public class MainSkillUI : MonoBehaviour
{
    [SerializeField] MainSkill skill;

    TextMeshProUGUI text;

    private void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetSkill(MainSkill skill)
    {
        this.skill = skill;
        text.text = skill.skillName;
    }
}
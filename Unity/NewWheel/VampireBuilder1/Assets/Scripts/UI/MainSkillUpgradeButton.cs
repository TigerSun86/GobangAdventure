using TMPro;
using UnityEngine;

public class MainSkillUpgradeButton : MonoBehaviour
{
    [SerializeField] MainSkill skill;

    TextMeshProUGUI buttonText;

    private void OnEnable()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetSkill(MainSkill skill)
    {
        this.skill = skill;
        buttonText.text = skill.skillName;
    }

    public void Upgrade()
    {
        skill.Enable();
    }
}

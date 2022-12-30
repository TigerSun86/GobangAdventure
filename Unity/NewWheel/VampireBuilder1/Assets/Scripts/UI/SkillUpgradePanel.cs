using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradePanel : MonoBehaviour
{
    [SerializeField] MainSkill skill;

    [SerializeField] MainSkillUpgradeButton mainSkillButton;

    [SerializeField] List<SubSkillUpgradeButton> subSkillButtons;

    public void SetSkill(MainSkill skill)
    {
        this.skill = skill;

        mainSkillButton.SetSkill(skill);

        int index = 0;
        foreach (SubSkill subSkill in skill.GetActiveSubSkills())
        {
            if (index == subSkillButtons.Count)
            {
                Debug.LogError($"Too many active sub skills: {index + 1}");
                break;
            }

            subSkillButtons[index].SetSkill(subSkill);
            index++;
        }
    }
}

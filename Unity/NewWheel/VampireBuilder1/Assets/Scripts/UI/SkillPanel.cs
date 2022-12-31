using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] MainSkill skill;

    [SerializeField] MainSkillUI mainSkillUI;

    [SerializeField] List<SubSkillUI> subSkillUIs;

    public void SetSkill(MainSkill skill)
    {
        this.skill = skill;

        mainSkillUI.SetSkill(skill);

        List<SubSkill> activeSubSkills = skill.GetActiveSubSkills().ToList();
        for (int i = 0; i < subSkillUIs.Count; i++)
        {
            SubSkill subSkill = null;
            if (i < activeSubSkills.Count)
            {
                subSkill = activeSubSkills[i];
            }

            subSkillUIs[i].SetSkill(subSkill);
        }
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}

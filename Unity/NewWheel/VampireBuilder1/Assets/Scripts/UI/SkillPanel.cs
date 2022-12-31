using System.Collections.Generic;
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

        int index = 0;
        foreach (SubSkill subSkill in skill.GetActiveSubSkills())
        {
            if (index == subSkillUIs.Count)
            {
                Debug.LogError($"Too many active sub skills: {index + 1}");
                break;
            }

            subSkillUIs[index].SetSkill(subSkill);
            index++;
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

    public void Initialize()
    {
        Enable();
        mainSkillUI.Initialize();

        foreach (SubSkillUI subSkillUI in subSkillUIs)
        {
            subSkillUI.Initialize();
        }

        Disable();
    }
}

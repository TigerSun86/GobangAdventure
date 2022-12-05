using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillUpgradeButton : MonoBehaviour
{
    [SerializeField] SkillBase skill;
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI skillDescriptionText;

    public void SetSkill(SkillBase skill)
    {
        this.skill = skill;
        skillNameText.text = skill.GetName();
        if (skill.dependencies.Any())
        {
            string dependencies = string.Join(",", skill.dependencies);
            skillNameText.text += $" ({dependencies})";
        }

        levelText.text = "Level " + skill.GetNextLevel();
        skillDescriptionText.text = skill.GetNextLevelDescription();
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
        skill.LevelUp();
    }
}

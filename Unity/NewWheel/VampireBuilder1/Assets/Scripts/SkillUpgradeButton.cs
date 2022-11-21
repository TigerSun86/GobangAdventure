using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillUpgradeButton : MonoBehaviour
{
    [SerializeField] SkillData skillData;
    [SerializeField] TextMeshProUGUI skillNameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI skillDescriptionText;

    public void SetSkill(SkillData skillData)
    {
        this.skillData = skillData;
        skillNameText.text = skillData.GetName();
        levelText.text = "Level " + skillData.GetNextLevel();
        skillDescriptionText.text = skillData.GetNextLevelDescription();
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
        skillData.LevelUp();
    }
}

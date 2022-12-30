using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeMenuV3Manager : MonoBehaviour
{
    [SerializeField] MainSkillRuntimeSet activeSkills;

    [SerializeField] List<SkillUpgradePanel> skillButtons;

    private void Start()
    {
        RefreshMenu();
    }

    private void RefreshMenu()
    {
        for (int i = 0; i < activeSkills.Items.Count; i++)
        {
            if (i == skillButtons.Count)
            {
                Debug.LogError($"Too many active main skills: {i + 1}");
                break;
            }

            SkillUpgradePanel skillButton = skillButtons[i];
            MainSkill mainSkill = activeSkills.Items[i];
            skillButton.SetSkill(mainSkill);
        }
    }
}

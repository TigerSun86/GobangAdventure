using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;

    [SerializeField] List<SkillUpgradeButton> skillButtons;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] SkillIdGameEvent skillSelectedEvent;

    public void OpenMenu()
    {
        if (!panel.activeInHierarchy)
        {
            panel.SetActive(true);
        }

        RefreshMenu();
    }

    public void CloseMenu()
    {
        panel.SetActive(false);
    }

    private void RefreshMenu()
    {
        bool isFirstButton = true;
        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillUpgradeButton skillButton = skillButtons[i];
            if (i < upgradeOptionSequence.Items.Count)
            {
                UpgradeOption upgradeOption = upgradeOptionSequence.Items[i];
                skillButton.Enable();
                skillButton.SetSkill(upgradeOption);
                skillButton.RegisterOnClickAction(() =>
                {
                    CloseMenu();
                    skillSelectedEvent.Raise(upgradeOption.skillId);
                });
                if (isFirstButton)
                {
                    isFirstButton = false;
                    EventSystem.current.SetSelectedGameObject(skillButtons[0].gameObject);
                }
            }
            else
            {
                skillButton.Disable();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeMenuManager : MonoBehaviour
{
    private readonly object upgradeVariablesLock = new object();

    [SerializeField] IntVariable pendingUpgradeCount;

    [SerializeField] GameObject panel;

    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    [SerializeField] List<SkillUpgradeButton> skillButtons;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] UnityEvent pendingUpgradeChangeEvent;

    public void OpenMenu()
    {
        bool needRefresh = false;
        lock (upgradeVariablesLock)
        {
            pendingUpgradeCount.ApplyChange(1);
            if (!panel.activeInHierarchy)
            {
                panel.SetActive(true);
                needRefresh = true;
            }

            // Else, there is another upgrade waiting for user to select, no need to refresh.
        }

        pendingUpgradeChangeEvent.Invoke();

        if (needRefresh)
        {
            RefreshMenu();
        }
    }

    public void CloseMenu()
    {
        bool needRefresh = false;
        lock (upgradeVariablesLock)
        {
            pendingUpgradeCount.ApplyChange(-1);
            if (pendingUpgradeCount.value <= 0)
            {
                panel.SetActive(false);
            }
            else
            {
                // Still have pending upgrades.
                needRefresh = true;
            }
        }

        pendingUpgradeChangeEvent.Invoke();

        if (needRefresh)
        {
            RefreshMenu();
        }
    }

    private void RefreshMenu()
    {
        bool isFirstButton = true;
        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillUpgradeButton skillButton = skillButtons[i];
            if (i < upgradeOptionSequence.Items.Count)
            {
                skillButton.Enable();
                skillButton.SetSkill(upgradeOptionSequence.Items[i]);
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

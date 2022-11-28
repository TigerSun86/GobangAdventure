using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeMenuManager : MonoBehaviour
{
    private readonly object upgradeVariablesLock = new object();

    [SerializeField] IntVariable pendingUpgradeCount;

    [SerializeField] GameObject panel;

    [SerializeField] SkillRuntimeSet skillPrefabs;

    [SerializeField] SkillRuntimeSet skillInstances;

    [SerializeField] List<SkillUpgradeButton> skillButtons;

    // Start is called before the first frame update
    void Start()
    {
        skillInstances.Items.Clear();
        foreach (SkillBase skillPrefab in skillPrefabs.Items)
        {
            skillInstances.Add(Instantiate(skillPrefab));
        }
    }

    public void OpenMenu()
    {
        lock (upgradeVariablesLock)
        {
            pendingUpgradeCount.ApplyChange(1);
            if (panel.activeInHierarchy)
            {
                // There is another upgrade waiting for user to select.
                return;
            }

            panel.SetActive(true);
        }

        RefreshMenu();
    }

    public void CloseMenu()
    {
        lock (upgradeVariablesLock)
        {
            pendingUpgradeCount.ApplyChange(-1);
            if (pendingUpgradeCount.value <= 0)
            {
                panel.SetActive(false);
                return;
            }
        }

        // Still have pending upgrades.
        RefreshMenu();
    }

    private void RefreshMenu()
    {
        bool isFirstButton = true;
        List<SkillBase> selectedSkills = skillInstances.Items.Where(s => s.IsUpgradable()).ToList();
        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillUpgradeButton skillButton = skillButtons[i];
            if (i < selectedSkills.Count)
            {
                skillButton.Enable();
                skillButton.SetSkill(selectedSkills[i]);
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

        // Invoke event
    }
}

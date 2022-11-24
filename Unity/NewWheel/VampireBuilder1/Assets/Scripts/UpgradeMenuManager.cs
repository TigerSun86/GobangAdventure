using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;

    [SerializeField] bool isEnabled = true;

    [SerializeField] SkillRuntimeSet skillPrefabs;

    [SerializeField] SkillRuntimeSet skillInstances;

    [SerializeField] List<SkillUpgradeButton> skillButtons;

    private GamePause gamePause;

    // Start is called before the first frame update
    void Start()
    {
        gamePause = GetComponent<GamePause>();
        Level playerLevel = Manager.instance.PlayerLevel;
        playerLevel.OnLevelUp.AddListener((level) => OpenMenu());

        skillInstances.Items.Clear();
        foreach (SkillBase skillPrefab in skillPrefabs.Items)
        {
            skillInstances.Add(Instantiate(skillPrefab));
        }
    }

    public void OpenMenu()
    {
        if (isEnabled)
        {
            gamePause.Pause();
            panel.SetActive(true);
            List<SkillBase> selectedSkills = skillInstances.Items.Where(s => s.IsUpgradable()).ToList();
            for (int i = 0; i < skillButtons.Count; i++)
            {
                SkillUpgradeButton skillButton = skillButtons[i];
                if (i < selectedSkills.Count)
                {
                    skillButton.Enable();
                    skillButton.SetSkill(selectedSkills[i]);
                }
                else
                {
                    skillButton.Disable();
                }
            }
        }
    }

    public void CloseMenu()
    {
        gamePause.Unpause();
        panel.SetActive(false);
    }
}

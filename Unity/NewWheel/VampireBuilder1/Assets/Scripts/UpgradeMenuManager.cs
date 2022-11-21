using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuManager : MonoBehaviour
{
    [SerializeField] GameObject panel;

    [SerializeField] bool isEnabled = true;

    [SerializeField] SkillRuntimeSet skills;

    [SerializeField] List<SkillUpgradeButton> skillButtons;

    private GamePause gamePause;

    // Start is called before the first frame update
    void Start()
    {
        gamePause = GetComponent<GamePause>();
        Level playerLevel = Manager.instance.PlayerLevel;
        playerLevel.OnLevelUp.AddListener((level) => OpenMenu());
    }

    public void OpenMenu()
    {
        if (isEnabled)
        {
            gamePause.Pause();
            panel.SetActive(true);
            for (int i = 0; i < skillButtons.Count; i++)
            {
                SkillUpgradeButton skillButton = skillButtons[i];
                if (i < skills.Items.Count)
                {
                    skillButton.Enable();
                    skillButton.SetSkill(skills.Items[i]);
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

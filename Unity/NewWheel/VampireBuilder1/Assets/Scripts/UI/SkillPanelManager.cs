using System.Collections.Generic;
using UnityEngine;

public class SkillPanelManager : MonoBehaviour
{
    [SerializeField] List<SkillPanel> skillPanels;

    [SerializeField] MainSkillRuntimeSet activeSkills;

    public void Refresh()
    {
        for (int i = 0; i < skillPanels.Count; i++)
        {
            SkillPanel skillPanel = skillPanels[i];
            if (i >= activeSkills.Items.Count)
            {
                skillPanel.Disable();
            }
            else
            {
                skillPanel.Enable();
                skillPanel.SetSkill(activeSkills.Items[i]);
            }
        }
    }
}

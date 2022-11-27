using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ExperienceBar : MonoBehaviour
{
    [SerializeField] Slider slider;

    [SerializeField] TMPro.TextMeshProUGUI levelText;

    [SerializeField] IntVariable levelValue;

    private void Start()
    {
        Level playerLevel = Manager.instance.PlayerLevel;
        playerLevel.OnExperienceChanged.AddListener(UpdateExperienceBar);
        UpdateExperienceBar(playerLevel.Experience, playerLevel.ToLevelUp);
        UpdateLevelText();
    }

    public void UpdateExperienceBar(int experienceValue, int experienceMax)
    {
        slider.value = experienceValue;
        slider.maxValue = experienceMax;
    }

    public void UpdateLevelText()
    {
        levelText.text = "Level: " + levelValue.value;
    }
}

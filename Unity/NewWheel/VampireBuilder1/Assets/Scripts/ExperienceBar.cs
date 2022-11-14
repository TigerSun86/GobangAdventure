using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ExperienceBar : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMPro.TextMeshProUGUI levelText;

    private void Awake()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Level playerLevel = playerObject.GetComponent<Level>();
        playerLevel.OnExperienceChanged.AddListener(UpdateExperienceBar);
        playerLevel.OnLevelUp.AddListener(UpdateLevelText);
    }

    public void UpdateExperienceBar(int experienceValue, int experienceMax)
    {
        slider.value = experienceValue;
        slider.maxValue = experienceMax;
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "Level: " + level;
    }
}

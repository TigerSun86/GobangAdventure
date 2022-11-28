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

    [SerializeField] IntVariable experianceValue;

    [SerializeField] IntVariable experianceToLevelUpValue;

    private void Start()
    {
        UpdateExperienceBar();
        UpdateLevelText();
    }

    public void UpdateExperienceBar()
    {
        slider.value = experianceValue.value;
        slider.maxValue = experianceToLevelUpValue.value;
    }

    public void UpdateLevelText()
    {
        levelText.text = "Level: " + levelValue.value;
    }
}

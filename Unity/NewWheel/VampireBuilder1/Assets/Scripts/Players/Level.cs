using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    private readonly object experienceOperationLock = new object();

    [SerializeField] UnityEvent levelUpEvent;

    [SerializeField] UnityEvent experienceChangeEvent;

    [SerializeField] IntVariable levelValue;

    [SerializeField] IntVariable experienceValue;

    [SerializeField] IntVariable experienceToLevelUpValue;

    public void ExtractExperience(GameObject gameObject)
    {
        ExperienceSource experienceSource = gameObject.GetComponent<ExperienceSource>();
        if (experienceSource != null)
        {
            AddExperience(experienceSource.ExperienceValue);
        }
    }

    private void Awake()
    {
        experienceToLevelUpValue.SetValue(CalculateExperienceToLevelUp(levelValue.value));
    }

    private void AddExperience(int amount)
    {
        int levelUpCount = 0;
        int experienceToLevelUp = CalculateExperienceToLevelUp(levelValue.value + levelUpCount);
        lock (experienceOperationLock)
        {
            int newExperienceValue = experienceValue.value + amount;
            while (newExperienceValue >= experienceToLevelUp)
            {
                newExperienceValue -= experienceToLevelUp;
                levelUpCount++;
                experienceToLevelUp = CalculateExperienceToLevelUp(levelValue.value + levelUpCount);
            }

            experienceValue.SetValue(newExperienceValue);
            experienceToLevelUpValue.SetValue(experienceToLevelUp);
        }

        experienceChangeEvent.Invoke();

        if (levelUpCount > 0)
        {
            levelValue.ApplyChange(levelUpCount);
            while (levelUpCount > 0)
            {
                levelUpEvent.Invoke();
                levelUpCount--;
            }
        }
    }

    private int CalculateExperienceToLevelUp(int level)
    {
        return (int)(Mathf.Sqrt(level) * 200);
    }
}

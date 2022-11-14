using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public UnityEvent<int, int> OnExperienceChanged { get; } = new UnityEvent<int, int>();

    public UnityEvent<int> OnLevelUp { get; } = new UnityEvent<int>();

    public int LevelValue { get; set; } = 1;

    public int Experience { get; set; } = 0;

    public int ToLevelUp { get => LevelValue * 200; }

    public void ExtractExperience(GameObject gameObject)
    {
        ExperienceSource experienceSource = gameObject.GetComponent<ExperienceSource>();
        if (experienceSource != null)
        {
            AddExperience(experienceSource.ExperienceValue);
        }
    }

    private void AddExperience(int amount)
    {
        Experience += amount;
        if (Experience >= ToLevelUp)
        {
            Experience -= ToLevelUp;
            LevelValue++;
            OnLevelUp.Invoke(LevelValue);
        }

        OnExperienceChanged.Invoke(Experience, ToLevelUp);
    }
}

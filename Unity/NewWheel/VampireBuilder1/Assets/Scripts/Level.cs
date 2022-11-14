using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    private int experience = 0;
    private int level = 1;

    public int ToLevelUp { get => level * 200; }

    public UnityEvent<int, int> OnExperienceChanged { get; } = new UnityEvent<int, int>();

    public UnityEvent<int> OnLevelUp { get; } = new UnityEvent<int>();

    private void Awake()
    {
        OnExperienceChanged.Invoke(experience, ToLevelUp);
        OnLevelUp.Invoke(level);
    }

    public void ExtractExperience(GameObject gameObject)
    {
        Debug.Log("Called");
        ExperienceSource experienceSource = gameObject.GetComponent<ExperienceSource>();
        if (experienceSource != null)
        {
            Debug.Log("Add");
            AddExperience(experienceSource.ExperienceValue);
        }
    }

    private void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= ToLevelUp)
        {
            experience -= ToLevelUp;
            level++;
            OnLevelUp.Invoke(level);
        }

        OnExperienceChanged.Invoke(experience, ToLevelUp);
    }
}

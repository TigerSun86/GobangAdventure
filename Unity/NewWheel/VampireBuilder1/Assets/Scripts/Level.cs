using UnityEngine;
using UnityEngine.Events;

public class Level : MonoBehaviour
{
    public UnityEvent<int, int> OnExperienceChanged { get; } = new UnityEvent<int, int>();

    [SerializeField] UnityEvent levelUpEvent;

    [SerializeField] IntVariable levelValue;

    public int Experience { get; set; } = 0;

    public int ToLevelUp { get => levelValue.value * levelValue.value * 200; }

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
            levelValue.ApplyChange(1);
            levelUpEvent.Invoke();
        }

        OnExperienceChanged.Invoke(Experience, ToLevelUp);
    }
}

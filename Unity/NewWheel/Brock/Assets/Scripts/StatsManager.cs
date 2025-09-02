using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance { get; private set; }

    public float maxHealth;
    public float attack;

    public void ApplyItemStats(ItemConfig itemConfig)
    {
        this.maxHealth += itemConfig.maxHealth;
        this.attack += itemConfig.attack;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

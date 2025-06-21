using UnityEngine;
using UnityEngine.Events;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [SerializeField]
    UnityEvent countToBuyChangeEvent;

    [SerializeField, AssignedInCode]
    private float countToBuy;

    public int CountToBuy
    {
        get { return (int)countToBuy; }
    }

    public void IncreaseCountToBuy()
    {
        this.countToBuy += 0.5f;
        this.countToBuyChangeEvent.Invoke();
    }

    public void DecreaseCountToBuy()
    {
        this.countToBuy -= 1;
        this.countToBuyChangeEvent.Invoke();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            this.countToBuy = 2;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

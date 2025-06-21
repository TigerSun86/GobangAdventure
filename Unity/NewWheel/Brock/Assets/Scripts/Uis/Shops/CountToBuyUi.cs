using TMPro;
using UnityEngine;

public class CountToBuyUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] ItemDb itemDb;

    [SerializeField, AssignedInCode]
    MoneyManager moneyManager;

    public void SetCount()
    {
        SetCount(this.moneyManager.CountToBuy);
    }

    private void SetCount(int count)
    {
        text.text = "Count To Buy: " + count.ToString();
    }

    private void Start()
    {
        this.moneyManager = MoneyManager.Instance;
        SetCount();
    }
}

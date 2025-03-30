using TMPro;
using UnityEngine;

public class CountToBuyUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] ItemDb itemDb;

    public void SetCount()
    {
        SetCount(itemDb.CountToBuy);
    }

    private void SetCount(int count)
    {
        text.text = "Count To Buy: " + count.ToString();
    }

    private void Start()
    {
        SetCount();
    }
}

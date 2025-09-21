using TMPro;
using UnityEngine;

public class CountToBuyUi : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField, AssignedInCode]
    LootManager lootManager;

    public void UpdateText()
    {
        text.text = "Weapon Count To Buy: " + (int)this.lootManager.GetWeaponCount()
            + "\nItem Count To Buy: " + (int)this.lootManager.GetItemCount()
            + "\nGold Count: " + (int)this.lootManager.GetGoldCount();
    }

    private void Start()
    {
        this.lootManager = LootManager.Instance;
        UpdateText();
    }
}

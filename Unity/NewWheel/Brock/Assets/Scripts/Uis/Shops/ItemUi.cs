using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ItemUi : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI categoryText;
    [SerializeField] TextMeshProUGUI statsText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Button purchaseButton;

    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetCategory(int level, string category)
    {
        categoryText.text = "Level " + level + " " + category;
    }

    public void SetAttack(float attack)
    {
        statsText.text = "Attack: " + attack.ToString();
    }

    public void SetPrice(int price)
    {
        priceText.text = price.ToString();
    }

    public void OnItemPurchase(string itemName, UnityAction<string> action)
    {
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => action.Invoke(itemName));
        purchaseButton.onClick.AddListener(() => purchaseButton.gameObject.SetActive(false));
    }
}
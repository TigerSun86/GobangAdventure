using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Linq;

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

    public void SetSkills(SkillConfig[] skillConfigs)
    {
        statsText.text = string.Join("\n",
            skillConfigs.Select(skill => $"{skill.skillName}: {skill.value}\n {skill.description}"));
    }

    public void SetPrice(int price)
    {
        priceText.text = price.ToString();
    }

    public void OnItemPurchase(string itemName, UnityAction<string> action)
    {
        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => action.Invoke(itemName));
        purchaseButton.onClick.AddListener(() => DisablePurchaseButton());
    }

    public void DisablePurchaseButton()
    {
        purchaseButton.gameObject.SetActive(false);
    }
}
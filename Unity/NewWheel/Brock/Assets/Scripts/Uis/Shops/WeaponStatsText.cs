using System.Linq;
using UnityEngine;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class WeaponStatsText : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private int slotId;

    [SerializeField, AssignedInCode]
    private TMPro.TextMeshProUGUI text;

    public void SetSlotId(int slotId)
    {
        if (slotId < 0)
        {
            Debug.LogError("Slot ID cannot be negative.");
            return;
        }

        this.slotId = slotId;
    }

    private void Start()
    {
        this.text = GetComponent<TMPro.TextMeshProUGUI>();

        WeaponStatus weapon = ConfigDb.Instance.weaponInventory.GetBySlotId(slotId);
        if (weapon == null)
        {
            this.text.text = string.Empty;
            return;
        }

        WeaponConfig weaponConfig = weapon.GetShopItem().weaponConfig;
        this.text.text =
            $"Name: {weaponConfig.weaponName}\n" +
            $"Type: {weaponConfig.weaponBaseType}\n" +
            $"Level: {weaponConfig.level}\n" +
            $"Experience: {weapon.GetCurrentExperience()}/{weaponConfig.experienceToNextLevel}\n" +
            string.Join("\n",
            weaponConfig.GetSkills().ToList().Select(skill => $"{skill.skillName}: {skill.value}\n {skill.description}")); ;
    }
}

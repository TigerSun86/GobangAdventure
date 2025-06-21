using UnityEngine;

public class WeaponSuit : MonoBehaviour
{
    public WeaponConfig2 weaponConfig;
    public WeaponItem weaponItem;
    public WeaponStand weaponStand;
    public SkillActor skillActor;

    public WeaponBaseType weaponBaseType => weaponConfig1 != null ? weaponConfig1.weaponBaseType : weaponConfig.weaponBaseType;

    private WeaponConfig weaponConfig1;

    public SkillConfig[] GetSkills()
    {
        if (weaponConfig1 != null)
        {
            return weaponConfig1.skills;
        }
        return weaponConfig.GetSkills();
    }

    public int GetMaxHealth()
    {
        if (weaponConfig1 != null)
        {
            return weaponConfig1.health;
        }
        return weaponConfig.health;
    }

    public void Initialize(WeaponConfig weaponConfig)
    {
        this.weaponConfig1 = weaponConfig;
        InitializeInternal();
    }

    public void Initialize(WeaponConfig2 weaponConfig)
    {
        this.weaponConfig = weaponConfig;
        InitializeInternal();
    }


    public Health GetHealth()
    {
        return weaponStand.health;
    }

    private void InitializeInternal()
    {
        this.weaponItem = GetComponentInChildren<WeaponItem>();
        this.weaponItem.Initialize(this);
        this.weaponStand = GetComponentInChildren<WeaponStand>();
        this.weaponStand.Initialize(this);
        this.skillActor = GetComponentInChildren<SkillActor>();
        this.skillActor.Initialize(this);
    }
}

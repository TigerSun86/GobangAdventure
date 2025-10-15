using UnityEngine;

public class WeaponSuit : MonoBehaviour
{
    public WeaponConfig weaponConfig;

    public WeaponItem weaponItem;

    public WeaponStand weaponStand;

    public SkillActor skillActor;

    public WeaponBaseType weaponBaseType => weaponConfig.weaponBaseType;

    public CapabilityController capabilityController;

    public PropertyController propertyController;

    public ModifierContainer modifierContainer;

    public void Initialize(WeaponConfig weaponConfig)
    {
        this.weaponConfig = weaponConfig;
        this.weaponItem = GetComponentInChildren<WeaponItem>();
        this.weaponItem.Initialize(this);
        this.weaponStand = GetComponentInChildren<WeaponStand>();
        this.weaponStand.Initialize(this);
        this.capabilityController = GetComponent<CapabilityController>();
        this.propertyController = GetComponent<PropertyController>();
        this.skillActor = GetComponentInChildren<SkillActor>();
        this.modifierContainer = GetComponent<ModifierContainer>();
    }

    public Health GetHealth()
    {
        return weaponStand.health;
    }

    private void Start()
    {
        this.skillActor.Initialize(this);
    }
}

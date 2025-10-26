using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierContainer : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private List<Modifier> modifiers = new List<Modifier>();

    private WeaponSuit weaponSuit;

    private PropertyController propertyController;

    private CriticalHitController criticalHitController;

    private StateController stateController;

    private TargetingSkillModifierController targetingSkillModifierController;

    private BuffUiPanel buffUiPanel;

    public void AddModifier(Modifier modifier)
    {
        modifier.Link(this.weaponSuit);
        this.modifiers.Add(modifier);
        NotifyDirty();
    }

    public void RemoveModifier(string id)
    {
        Modifier modifier = this.modifiers.FirstOrDefault(m => m.config.id == id);
        if (modifier == null)
        {
            Debug.LogError($"Could not find the modifier with id {id}");
            return;
        }

        RemoveModifier(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        modifier.Unlink();
        this.modifiers.Remove(modifier);
        NotifyDirty();
    }

    public IEnumerable<Modifier> GetAllModifiers()
    {
        return this.modifiers;
    }

    private void Awake()
    {
        this.weaponSuit = GetComponent<WeaponSuit>();
        this.propertyController = GetComponent<PropertyController>();
        this.criticalHitController = GetComponent<CriticalHitController>();
        this.stateController = GetComponent<StateController>();
        this.targetingSkillModifierController = GetComponent<TargetingSkillModifierController>();
        this.buffUiPanel = GetComponent<BuffUiPanel>();
    }

    private void FixedUpdate()
    {
        List<Modifier> toRemove = new List<Modifier>();
        foreach (Modifier modifier in this.modifiers)
        {
            if (!modifier.Tick(Time.fixedDeltaTime))
            {
                toRemove.Add(modifier);
            }
        }

        foreach (Modifier modifier in toRemove)
        {
            RemoveModifier(modifier);
        }
    }

    private void NotifyDirty()
    {
        this.propertyController.NotifyDirty();
        this.criticalHitController.NotifyDirty();
        this.stateController.NotifyDirty();
        this.targetingSkillModifierController.NotifyDirty();
        this.buffUiPanel.NotifyDirty();
    }
}
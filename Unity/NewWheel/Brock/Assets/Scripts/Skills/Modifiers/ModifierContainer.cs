using System.Collections.Generic;
using UnityEngine;

public class ModifierContainer : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private List<Modifier> modifiers = new List<Modifier>();

    private WeaponSuit weaponSuit;

    private PropertyController propertyController;

    private StateController stateController;

    private BuffUiPanel buffUiPanel;

    public void AddModifier(Modifier modifier)
    {
        modifier.Link(this.weaponSuit);
        this.modifiers.Add(modifier);
        NotifyDirty();
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
        this.stateController = GetComponent<StateController>();
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
        this.stateController.NotifyDirty();
        this.buffUiPanel.NotifyDirty();
    }
}
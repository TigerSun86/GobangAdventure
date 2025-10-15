using System.Collections.Generic;
using UnityEngine;

public class ModifierContainer : MonoBehaviour
{
    [SerializeField, AssignedInCode]
    private List<Modifier> modifiers = new List<Modifier>();

    private PropertyController propertyController;

    private BuffUiPanel buffUiPanel;

    public void AddModifier(Modifier modifier)
    {
        this.modifiers.Add(modifier);
        NotifyDirty();
    }

    public void RemoveModifier(Modifier modifier)
    {
        this.modifiers.Remove(modifier);
        NotifyDirty();
    }

    public IEnumerable<Modifier> GetAllModifiers()
    {
        return this.modifiers;
    }

    private void Awake()
    {
        this.propertyController = GetComponent<PropertyController>();
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
        this.buffUiPanel.NotifyDirty();
    }
}
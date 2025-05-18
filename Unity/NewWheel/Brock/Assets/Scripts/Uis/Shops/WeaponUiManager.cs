using UnityEngine;
using UnityEngine.UI;

public class WeaponUiManager : MonoBehaviour
{
    public static WeaponUiManager Instance;

    [AssignedInCode] public WeaponSuit currentlyDragging;
    [AssignedInCode] public PulseOnHover currentHoverTarget;

    private Selectable[] allSelectable;

    public bool IsHiding { get; private set; }

    public void HideAllOtherSelectables(Selectable[] selectables)
    {
        if (IsHiding)
        {
            Debug.LogError("Already hiding selectables");
            return;
        }

        IsHiding = true;

        allSelectable = Selectable.allSelectablesArray;

        foreach (Selectable selectable in allSelectable)
        {
            selectable.interactable = false;
        }

        foreach (Selectable selectable in selectables)
        {
            selectable.interactable = true;
        }
    }

    public void UnhideSelectables()
    {
        if (!IsHiding)
        {
            Debug.LogError("Not hiding selectables");
            return;
        }

        IsHiding = false;

        if (allSelectable == null)
        {
            Debug.LogError("Should run HideAllOtherSelectables before this method");
            return;
        }

        foreach (Selectable selectable in allSelectable)
        {
            if (selectable != null && !selectable.IsDestroyed())
            {
                selectable.interactable = true;
            }
        }

        allSelectable = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetHoverTarget(PulseOnHover npc)
    {
        currentHoverTarget = npc;
    }

    public void ClearHoverTarget(PulseOnHover npc)
    {
        if (currentHoverTarget == npc)
            currentHoverTarget = null;
    }
}
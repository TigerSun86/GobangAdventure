using UnityEngine;

public class WeaponUiManager : MonoBehaviour
{
    public static WeaponUiManager Instance;

    [AssignedInCode] public Draggable currentlyDragging;
    [AssignedInCode] public PulseOnHover currentHoverTarget;

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
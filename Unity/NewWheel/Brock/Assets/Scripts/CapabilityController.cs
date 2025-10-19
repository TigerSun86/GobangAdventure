using UnityEngine;

[RequireComponent(typeof(StateController))]
public class CapabilityController : MonoBehaviour
{
    private StateController stateController;

    public bool Can(CapabilityType capability)
    {
        switch (capability)
        {
            case CapabilityType.Move:
                return !this.stateController.IsEnabled(ModifierStateType.MODIFIER_STATE_STUNNED);
            case CapabilityType.Attack:
                return !this.stateController.IsEnabled(ModifierStateType.MODIFIER_STATE_STUNNED);
            case CapabilityType.CastSkill:
                return !this.stateController.IsEnabled(ModifierStateType.MODIFIER_STATE_STUNNED);
            default:
                Debug.LogWarning($"Capability {capability} is not explicitly handled. Defaulting to true.");
                return true;
        }
    }

    private void Awake()
    {
        this.stateController = GetComponent<StateController>();
    }
}
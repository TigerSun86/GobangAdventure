using UnityEngine;

[RequireComponent(typeof(BuffTracker))]
public class CapabilityController : MonoBehaviour
{
    private BuffTracker buffTracker;

    public bool Can(CapabilityType capability)
    {
        switch (capability)
        {
            case CapabilityType.Move:
                return !this.buffTracker.Contains(BuffType.Stun);
            case CapabilityType.Attack:
                return !this.buffTracker.Contains(BuffType.Stun);
            case CapabilityType.CastSkill:
                return !this.buffTracker.Contains(BuffType.Stun);
            default:
                Debug.LogWarning($"Capability {capability} is not explicitly handled. Defaulting to true.");
                return true;
        }
    }

    private void Awake()
    {
        this.buffTracker = GetComponent<BuffTracker>();
    }
}
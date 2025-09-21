using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
    [SerializeField, Required]
    private GameObject weaponSlotUiPrefab;

    [SerializeField, AssignedInCode]
    private GameObject weaponSlotUiInstance;

    [SerializeField, AssignedInCode]
    private int slotId;

    public void Initialize(int slotId)
    {
        if (slotId < 0)
        {
            Debug.LogError("Slot ID cannot be negative.");
            return;
        }

        this.slotId = slotId;
    }

    public int GetSlotId()
    {
        return this.slotId;
    }

    public WeaponSuit GetWeaponSuit()
    {
        return GetComponentInChildren<WeaponSuit>();
    }

    void Start()
    {
        if (WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        this.weaponSlotUiInstance = Instantiate(this.weaponSlotUiPrefab, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
        this.weaponSlotUiInstance.SetActive(false);
    }

    void Update()
    {
        if (WaveManager.Instance.IsWaveRunning || this.weaponSlotUiInstance == null)
        {
            return;
        }

        if (this.GetWeaponSuit() != null || WeaponUiManager.Instance.IsDragging())
        {
            this.weaponSlotUiInstance.SetActive(true);
        }
        else
        {
            this.weaponSlotUiInstance.SetActive(false);
        }
    }
}

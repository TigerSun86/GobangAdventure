using UnityEngine;

public class WeaponLayout : MonoBehaviour
{
    [SerializeField]
    private int weaponSlotCount;

    [SerializeField]
    private string weaponTag;

    [SerializeField, Required]
    private GameObject weaponSlotPrefab;

    [SerializeField, Required]
    private GameObject weaponSuitPrefab;

    [SerializeField, AssignedInCode]
    private GameObject[] weaponSlots;

    [SerializeField, AssignedInCode]
    private GameObject[] weaponSuits;

    [SerializeField, AssignedInCode]
    private WeaponConfig[] weaponConfigs;

    public int GetWeaponSlotCount()
    {
        return weaponSlotCount;
    }

    public int GetWeaponSlotIdByWeapon(WeaponSuit weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("Weapon or target slot is null");
            return -1;
        }

        int sourceId = GetWeaponSlotIdBySlot(weapon.transform.parent.gameObject);
        return sourceId;
    }

    public int GetWeaponSlotIdBySlot(GameObject weaponSlot)
    {
        for (int i = 0; i < this.weaponSlots.Length; i++)
        {
            if (this.weaponSlots[i] == weaponSlot)
            {
                return i;
            }
        }

        return -1;
    }

    public void SetWeaponConfig(int weaponSlotId, WeaponConfig weaponConfig)
    {
        if (weaponSlotId < 0 || weaponSlotId >= this.weaponConfigs.Length)
        {
            Debug.LogError("Invalid weapon slot ID: " + weaponSlotId);
            return;
        }

        this.weaponConfigs[weaponSlotId] = weaponConfig;
    }

    public void RefreshWeapons()
    {
        DestroyAllWeapons();
        this.weaponSuits = new GameObject[this.weaponSlots.Length];
        for (int id = 0; id < this.weaponSlots.Length; id++)
        {
            WeaponConfig weaponConfig = weaponConfigs[id];
            if (weaponConfig == null)
            {
                continue;
            }

            Vector3 position = this.weaponSlots[id].transform.position;
            this.weaponSuits[id] = Instantiate(weaponSuitPrefab, position, Quaternion.identity, this.weaponSlots[id].transform);
            this.weaponSuits[id].tag = weaponTag;
            this.weaponSuits[id].GetComponent<WeaponSuit>().Initialize(weaponConfig);
        }
    }

    public void SwapWeapon(int sourceId, int targetId)
    {
        if (sourceId < 0 || sourceId >= this.weaponSlots.Length || targetId < 0 || targetId >= this.weaponSlots.Length)
        {
            Debug.LogError("Invalid source or target slot ID.");
            return;
        }

        GameObject sourceSlot = this.weaponSlots[sourceId];
        GameObject targetSlot = this.weaponSlots[targetId];

        GameObject sourceWeapon = this.weaponSuits[sourceId];
        GameObject targetWeapon = this.weaponSuits[targetId];

        if (sourceWeapon != null)
        {
            sourceWeapon.transform.SetParent(targetSlot.transform);
            sourceWeapon.transform.position = targetSlot.transform.position;
        }

        if (targetWeapon != null)
        {
            targetWeapon.transform.SetParent(sourceSlot.transform);
            targetWeapon.transform.position = sourceSlot.transform.position;
        }

        this.weaponSuits[sourceId] = targetWeapon;
        this.weaponSuits[targetId] = sourceWeapon;
    }

    public bool AreNeighbours(WeaponSuit weapon1, WeaponSuit weapon2)
    {
        if (weapon1 == null || weapon2 == null)
        {
            return false;
        }

        int slotId1 = GetWeaponSlotIdByWeapon(weapon1);
        int slotId2 = GetWeaponSlotIdByWeapon(weapon2);
        if (slotId1 < 0 || slotId2 < 0 || slotId1 >= this.weaponSlots.Length || slotId2 >= this.weaponSlots.Length)
        {
            return false;
        }

        int difference = Mathf.Abs(slotId1 - slotId2);
        return difference == 1 || difference == this.weaponSlots.Length - 1;
    }

    private void Awake()
    {
        this.weaponConfigs = new WeaponConfig[this.weaponSlotCount];

        float radius = GetGameObjectRadius();
        Vector2[] offsets = GetWeaponSlotOffsets(radius, this.weaponSlotCount);
        this.weaponSlots = new GameObject[offsets.Length];
        for (int i = 0; i < offsets.Length; i++)
        {
            Vector3 position = (Vector2)this.transform.position + offsets[i];
            this.weaponSlots[i] = Instantiate(this.weaponSlotPrefab, position, Quaternion.identity, this.transform);
        }
    }

    private void DestroyAllWeapons()
    {
        if (this.weaponSuits == null)
        {
            return;
        }

        foreach (GameObject weapon in this.weaponSuits)
        {
            if (weapon != null)
            {
                // Only destroy is not enough because it happens in the end of the frame.
                // So we need to disable it first to avoid FindGameObjectsWithTag returning it.
                weapon.SetActive(false);
                Destroy(weapon);
            }
        }
    }

    private float GetGameObjectRadius()
    {
        float radius = this.transform.localScale.x / 2f;
        return radius;
    }

    private Vector2[] GetWeaponSlotOffsets(float radius, int count)
    {
        Vector2[] offsets = new Vector2[count];
        for (int i = 0; i < count; i++)
        {
            float angle = i * 2 * Mathf.PI / count;
            offsets[i] = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
        }
        return offsets;
    }
}

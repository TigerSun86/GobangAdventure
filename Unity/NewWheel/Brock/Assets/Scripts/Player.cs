using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField, Required]
    private WeaponLayout weaponLayout;

    private Rigidbody2D rb;

    private WeaponInventory weaponInventory;

    public void RefreshWeapons()
    {
        RefreshWeaponConfigs();
        this.weaponLayout.RefreshWeapons();
    }

    public void SwapWeapon(int sourceSlotId, int targetSlotId)
    {
        this.weaponInventory.Swap(sourceSlotId, targetSlotId);
        this.weaponLayout.SwapWeapon(sourceSlotId, targetSlotId);
        RefreshWeaponConfigs();
    }

    private void Start()
    {
        if (WaveManager.Instance.IsWaveRunning)
        {
            this.rb = GetComponent<Rigidbody2D>();
        }

        this.weaponInventory = ConfigDb.Instance.weaponInventory;
        this.weaponInventory.SetMaxStorage(this.weaponLayout.GetWeaponSlotCount());

        RefreshWeapons();
    }

    private void FixedUpdate()
    {
        if (!WaveManager.Instance.IsWaveRunning)
        {
            return;
        }

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        this.rb.linearVelocity = speed * move;
    }

    private void RefreshWeaponConfigs()
    {
        for (int id = 0; id < this.weaponLayout.GetWeaponSlotCount(); id++)
        {
            WeaponStatus weapon = this.weaponInventory.GetBySlotId(id);
            this.weaponLayout.SetWeaponConfig(id, weapon?.GetShopItem().weaponConfig);
        }
    }
}

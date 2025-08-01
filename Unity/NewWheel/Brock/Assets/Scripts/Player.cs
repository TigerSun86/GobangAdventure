using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    [SerializeField, Required]
    private WeaponLayout weaponLayout;

    private Rigidbody2D rb;

    private PlayerShopItemManager playerShopItemManager;

    public void RefreshWeapons()
    {
        RefreshWeaponConfigs();
        this.weaponLayout.RefreshWeapons();
    }

    public void SwapWeapon(WeaponSuit weapon, GameObject targetSlot)
    {
        int sourceId = this.weaponLayout.GetWeaponSlotIdByWeapon(weapon);
        int targetId = this.weaponLayout.GetWeaponSlotIdBySlot(targetSlot);
        this.playerShopItemManager.Swap(sourceId, targetId);
        this.weaponLayout.SwapWeapon(sourceId, targetId);
        RefreshWeaponConfigs();
    }

    private void Start()
    {
        if (WaveManager.Instance.IsWaveRunning)
        {
            this.rb = GetComponent<Rigidbody2D>();
        }

        this.playerShopItemManager = PlayerShopItemManager.Instance;
        this.playerShopItemManager.InitializeIfNeeded();
        this.playerShopItemManager.SetMaxStorage(this.weaponLayout.GetWeaponSlotCount());

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
            ShopItem shopItem = this.playerShopItemManager.Get(id);
            this.weaponLayout.SetWeaponConfig(id, shopItem?.weaponConfig);
        }
    }
}

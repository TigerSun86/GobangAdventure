using UnityEngine;

public class OnFaintingRouter : MonoBehaviour
{
    private IOnFaintingHandler[] handlers = { new ReviveWhenFaintingOnFaintingHandler() };

    public void Route(WeaponSuit weaponSuit)
    {
        foreach (IOnFaintingHandler handler in handlers)
        {
            handler.Handle(weaponSuit);
        }
    }
}

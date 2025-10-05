using UnityEngine;

public class OnTakenDamageRouter : MonoBehaviour
{
    private IOnTakenDamageHandler[] handlers = { new LifestealOnTakenDamageHandler() };

    public void Route(DamageData damageData)
    {
        foreach (IOnTakenDamageHandler handler in handlers)
        {
            handler.Handle(damageData);
        }
    }
}

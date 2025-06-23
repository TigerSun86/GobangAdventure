using UnityEngine;

public class BuffEffectManager : MonoBehaviour
{
    [SerializeField]
    private BuffTypeToGameObjectDictionary effectPrefabs;

    public void CreateEffect(GameObject buffedGameObject, Buff buff)
    {
        if (!effectPrefabs.TryGetValue(buff.buffType, out GameObject effectPrefab))
        {
            return;
        }

        GameObject effectInstance = Instantiate(effectPrefab, buffedGameObject.transform);
        Destroy(effectInstance, buff.duration);
    }
}

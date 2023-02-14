using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillAttributeManager skillAttributeManager;

    [SerializeField] SkillIdToGameObjectDictionary skillIdToPrefab;

    public void InstantiateSkillPrefabs(Collider2D other, GameObject bullet)
    {
        SkillStage1 skillStage1 = bullet.GetComponent<SkillStage1>();
        SkillId skillId = skillStage1.skillId;
        GameObject prefab = skillIdToPrefab[skillId];
        GameObject instance = Instantiate(
            prefab,
            other.gameObject.transform.position,
            Quaternion.identity,
            this.transform);
        SkillPrefab skillPrefab = instance.GetComponent<SkillPrefab>();
        skillPrefab.target = other.gameObject;
        skillPrefab.skillAttributeManager = skillAttributeManager;
    }
}

using UnityEngine;

public class SkillStage2Manager : MonoBehaviour
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
        SkillStage2 skillPrefab = instance.GetComponent<SkillStage2>();
        skillPrefab.target = other.gameObject;
        skillPrefab.skillAttributeManager = skillAttributeManager;
    }
}

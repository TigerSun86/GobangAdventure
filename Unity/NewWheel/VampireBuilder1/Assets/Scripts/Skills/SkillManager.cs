using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    [SerializeField] List<Skill> skills;

    [SerializeField] MainSkill initialMainSkill;

    [SerializeField] MainSkillRuntimeSet activeSkills;

    [SerializeField] MainSkillRuntimeSet inactiveSkills;

    [SerializeField] UpgradeOptionRuntimeSet upgradeOptionSequence;

    [SerializeField] List<SkillNameAndPrefab> skillNameAndPrefabs;

    Dictionary<string, GameObject> skillNameAndPrefabMap;

    public void RefreshSkillUpgradeSequence()
    {
        skillUpgradeSequence.Items.Clear();
        skillUpgradeSequence.Items.AddRange(
            skills
                .Where(s => s.IsUpgradable())
                .OrderBy(s => Random.value));

        List<UpgradeOption> upgradeOptions = new List<UpgradeOption>();

        foreach (MainSkill mainSkill in activeSkills.Items)
        {
            foreach (SubSkill subSkill in mainSkill.subSkills)
            {
                if (subSkill.CanLevelUp())
                {
                    upgradeOptions.Add(new SubSkillUpgradeOption(subSkill));
                }
            }
        }

        foreach (MainSkill mainSkill in inactiveSkills.Items)
        {
            upgradeOptions.Add(new MainSkillUpgradeOption(mainSkill));
        }

        upgradeOptionSequence.Items.Clear();
        upgradeOptionSequence.Items.AddRange(upgradeOptions.OrderBy(s => Random.value));
    }

    public void InstantiateSkillPrefabs(Collider2D other, GameObject bullet)
    {
        foreach (MainSkill mainSkill in activeSkills.Items)
        {
            if (!skillNameAndPrefabMap.ContainsKey(mainSkill.skillName))
            {
                Debug.LogError($"Could not find prefab for skill [{mainSkill.skillName}]");
                continue;
            }

            GameObject prefab = skillNameAndPrefabMap[mainSkill.skillName];
            GameObject instance = Instantiate(
                prefab,
                other.gameObject.transform.position,
                Quaternion.identity,
                this.transform);
            instance.GetComponent<SkillPrefab>().target = other.gameObject;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        skillNameAndPrefabMap = new Dictionary<string, GameObject>();
        foreach (SkillNameAndPrefab pair in skillNameAndPrefabs)
        {
            skillNameAndPrefabMap[pair.skillName] = pair.prefab;
        }

        activeSkills.Clear();
        initialMainSkill.Enable();

        RefreshSkillUpgradeSequence();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField] SkillRuntimeSet skillPrefabs;

    [SerializeField] SkillRuntimeSet skillInstances;

    [SerializeField] SkillRuntimeSet skillUpgradeSequence;

    public void RefreshSkillUpgradeSequence()
    {
        skillUpgradeSequence.Items.Clear();
        skillUpgradeSequence.Items.AddRange(skillInstances.Items.Where(s => s.IsUpgradable()));
    }

    // Start is called before the first frame update
    void Start()
    {
        skillInstances.Items.Clear();
        foreach (SkillBase skillPrefab in skillPrefabs.Items)
        {
            skillInstances.Add(Instantiate(skillPrefab, Vector3.zero, Quaternion.identity, this.transform));
        }

        RefreshSkillUpgradeSequence();
    }
}

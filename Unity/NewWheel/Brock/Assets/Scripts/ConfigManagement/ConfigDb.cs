using System.Collections.Generic;
using UnityEngine;

public class ConfigDb : MonoBehaviour
{
    [SerializeField, Required]
    TextAsset skillConfigCsv;

    [SerializeField, Required]
    TextAsset weaponConfigCsv;

    [SerializeField, AssignedInCode]
    public SkillConfigDb skillConfigDb;

    [SerializeField, AssignedInCode]
    public WeaponConfigDb weaponConfigDb;

    public static ConfigDb Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            List<SkillConfig> skillConfigs = CsvLoader.LoadFromCSV(skillConfigCsv, new SkillConfigParser());
            this.skillConfigDb = new SkillConfigDb(skillConfigs);
            List<WeaponConfig2> weaponConfigs = CsvLoader.LoadFromCSV(weaponConfigCsv, new WeaponConfigParser(this.skillConfigDb));
            this.weaponConfigDb = new WeaponConfigDb(weaponConfigs);

            Debug.Log($"Loaded {skillConfigs.Count} skills and {weaponConfigs.Count} weapons.");
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

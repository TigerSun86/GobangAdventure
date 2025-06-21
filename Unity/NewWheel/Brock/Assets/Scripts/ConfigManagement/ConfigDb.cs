using System.Collections.Generic;
using UnityEngine;

public class ConfigDb : MonoBehaviour
{
    [SerializeField, Required]
    TextAsset skillConfigCsv;

    [SerializeField, Required]
    TextAsset weaponConfigCsv;

    // Just for debugging in Unity Editor.
    public List<SkillConfig> skillConfigs;

    // Just for debugging in Unity Editor.
    public List<WeaponConfig2> weaponConfigs;

    [AssignedInCode]
    public SkillConfigDb skillConfigDb;

    [AssignedInCode]
    public WeaponConfigDb weaponConfigDb;

    public static ConfigDb Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            this.skillConfigs = CsvLoader.LoadFromCSV(skillConfigCsv, new SkillConfigParser());
            this.skillConfigDb = new SkillConfigDb(this.skillConfigs);
            this.weaponConfigs = CsvLoader.LoadFromCSV(weaponConfigCsv, new WeaponConfigParser(this.skillConfigDb));
            this.weaponConfigDb = new WeaponConfigDb(this.weaponConfigs);

            Debug.Log($"Loaded {this.skillConfigs.Count} skills and {this.weaponConfigs.Count} weapons.");
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

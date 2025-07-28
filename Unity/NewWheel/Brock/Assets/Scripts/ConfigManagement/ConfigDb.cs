using System.Collections.Generic;
using UnityEngine;

public class ConfigDb : MonoBehaviour
{
    [SerializeField, Required]
    TextAsset skillConfigCsv;

    [SerializeField, Required]
    TextAsset weaponConfigCsv;

    [SerializeField, Required]
    TextAsset enemyConfigCsv;

    [SerializeField, Required]
    TextAsset waveConfigCsv;

    [SerializeField, AssignedInCode]
    public SkillConfigDb skillConfigDb;

    [SerializeField, AssignedInCode]
    public WeaponConfigDb weaponConfigDb;

    [SerializeField, AssignedInCode]
    public EnemyConfigDb enemyConfigDb;

    [SerializeField, AssignedInCode]
    public WaveConfigDb waveConfigDb;

    public static ConfigDb Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            List<SkillConfig> skillConfigs = CsvLoader.LoadFromCSV(skillConfigCsv, new SkillConfigParser());
            this.skillConfigDb = new SkillConfigDb(skillConfigs);
            List<WeaponConfig> weaponConfigs = CsvLoader.LoadFromCSV(weaponConfigCsv, new WeaponConfigParser(this.skillConfigDb));
            this.weaponConfigDb = new WeaponConfigDb(weaponConfigs);
            List<EnemyConfig> enemyConfigs = CsvLoader.LoadFromCSV(enemyConfigCsv, new EnemyConfigParser(this.weaponConfigDb));
            this.enemyConfigDb = new EnemyConfigDb(enemyConfigs);
            List<EnemyInWaveConfig> waveConfigs = CsvLoader.LoadFromCSV(waveConfigCsv, new WaveConfigParser(this.enemyConfigDb));
            this.waveConfigDb = new WaveConfigDb(waveConfigs);

            Debug.Log($"Loaded {skillConfigs.Count} skills, {weaponConfigs.Count} weapons, {enemyConfigs.Count} enemies, {waveConfigs.Count} wave fleets.");
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
}

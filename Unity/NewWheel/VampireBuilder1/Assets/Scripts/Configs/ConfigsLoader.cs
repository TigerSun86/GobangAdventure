using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigsLoader : MonoBehaviour
{
    private static readonly string SKILL_CONFIG_FILE_NAME = "skill_tbskill";

    [SerializeField] TbSkillConfig tbSkillConfig;

    public void Load()
    {
        string json = File.ReadAllText(Application.dataPath + "/../GenerateDatas/json/" + SKILL_CONFIG_FILE_NAME + ".json", System.Text.Encoding.UTF8);

        tbSkillConfig.SetTbSkillConfig(Newtonsoft.Json.JsonConvert.DeserializeObject<List<SkillConfig>>(json));
    }
}

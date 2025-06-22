using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ConfigsLoader : MonoBehaviour
{
    private static readonly string SKILL_CONFIG_FILE_NAME = "tbskillconfig";

    [SerializeField] TbSkillConfig tbSkillConfig;

    public void Load()
    {
        string json = File.ReadAllText(Application.dataPath + "/../DesignerConfigs/GenerateDatas/json/" + SKILL_CONFIG_FILE_NAME + ".json", System.Text.Encoding.UTF8);

        tbSkillConfig.SetTbSkillConfig(JsonUtility.FromJson<List<SkillConfig>>(json));

        EditorUtility.SetDirty(tbSkillConfig);
    }
}

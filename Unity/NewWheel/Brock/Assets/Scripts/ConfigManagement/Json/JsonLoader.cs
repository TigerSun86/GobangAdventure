using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public static class JsonLoader
{
    static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        Converters = { new StringEnumConverter(), new ActionConfigConverter(), new ModifierConfigConverter() }
    };

    // Example path: "Configs/Skills"
    public static List<T> LoadAll<T>(string path)
    {
        List<T> result = new List<T>();

        TextAsset[] assets = Resources.LoadAll<TextAsset>(path);
        if (assets == null || assets.Length == 0)
        {
            Debug.LogError($"No JSON files found at path: {path}");
            return result;
        }

        foreach (TextAsset ta in assets)
        {
            try
            {
                T cfg = JsonConvert.DeserializeObject<T>(ta.text, Settings);
                result.Add(cfg);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to parse JSON in file '{ta.name}': {ex.Message} \n{ta.text}");
                continue;
            }
        }

        return result;
    }
}

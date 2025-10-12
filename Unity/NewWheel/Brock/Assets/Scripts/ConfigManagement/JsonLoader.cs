using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class JsonLoader
{
    static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
    {
        MissingMemberHandling = MissingMemberHandling.Error,
        NullValueHandling = NullValueHandling.Include,
        Converters = new List<JsonConverter> { }
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
                Debug.LogError($"Failed to parse JSON in file '{ta.name}': {ex.Message}");
                continue;
            }
        }

        return result;
    }
}

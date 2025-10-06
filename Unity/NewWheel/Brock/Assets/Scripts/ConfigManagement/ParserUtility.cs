using System;
using System.Collections.Generic;
using UnityEngine;

public static class ParserUtility
{
    private static readonly Dictionary<string, Sprite[]> SpritesCache = new Dictionary<string, Sprite[]>();

    public static void ValidateHeaders(HashSet<string> expectedHeaders, string[] validatedHeaders)
    {
        HashSet<string> seen = new HashSet<string>();

        foreach (string h in validatedHeaders)
        {
            string key = h.Trim();
            seen.Add(key);

            if (!expectedHeaders.Contains(key))
            {
                Debug.LogWarning($"Unrecognized column: '{h}'");
            }
        }

        foreach (string expected in expectedHeaders)
        {
            if (!seen.Contains(expected))
            {
                Debug.LogWarning($"Missing expected column: '{expected}'");
            }
        }
    }

    public static int ParseIntSafe(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        if (!int.TryParse(value, out int result))
        {
            Debug.LogWarning($"Failed to parse '{fieldName}' with value '{value}'");
        }

        return result;
    }

    public static float ParseFloatSafe(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        if (!float.TryParse(value, out float result))
        {
            Debug.LogWarning($"Failed to parse '{fieldName}' with value '{value}'");
        }

        return result;
    }

    public static T ParseEnum<T>(string value, bool ignoreCase = true, T defaultValue = default) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        if (Enum.TryParse<T>(value.Trim(), ignoreCase, out T result))
        {
            return result;
        }

        Debug.LogWarning($"Failed to parse '{value}' as {typeof(T).Name}. Using default: {defaultValue}");
        return defaultValue;
    }

    public static bool ParseBoolSafe(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (bool.TryParse(value, out bool result))
        {
            return result;
        }

        Debug.LogWarning($"Failed to parse '{fieldName}' with value '{value}'");
        return false;
    }

    public static Sprite ParseSpriteSafe(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Debug.LogError($"Sprite path for '{fieldName}' is empty.");
            return null;
        }

        // path[#subsprite]
        string[] parts = value.Split('#');
        string path = parts[0];
        if (parts.Length == 1)
        {
            Sprite sprite1 = Resources.Load<Sprite>(path);
            if (sprite1 == null)
            {
                Debug.LogError($"Sprite not found at path '{path}' for '{fieldName}'.");
            }

            return sprite1;
        }

        string sub = parts[1];

        // Load from sliced tileset
        if (!ParserUtility.SpritesCache.TryGetValue(path, out Sprite[] sprites))
        {
            sprites = Resources.LoadAll<Sprite>(path);
            if (sprites.Length == 0)
            {
                Debug.LogError($"No sprites found at path '{path}' for '{fieldName}'.");
                return null;
            }

            ParserUtility.SpritesCache[path] = sprites;
        }

        Sprite sprite = Array.Find(sprites, s => s.name == sub);
        if (sprite == null)
        {
            Debug.LogError($"Sub-sprite '{sub}' not found in '{path}'.");
            return null;
        }

        return sprite;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public static class ParserUtility
{
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
        if (!int.TryParse(value, out int result))
        {
            Debug.LogWarning($"Failed to parse '{fieldName}' with value '{value}'");
        }

        return result;
    }

    public static float ParseFloatSafe(string value, string fieldName)
    {
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
}
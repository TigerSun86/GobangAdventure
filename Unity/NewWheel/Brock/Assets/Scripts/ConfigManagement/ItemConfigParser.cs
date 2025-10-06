using System.Collections.Generic;
using UnityEngine;

public class ItemConfigParser : ICsvRowParser<ItemConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "itemName", "level", "price", "spritePath", "maxHealth", "attack"
    };

    private bool validated = false;

    public ItemConfigParser()
    {
    }

    public ItemConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        ItemConfig result = new ItemConfig();

        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string value = values[i].Trim();

            switch (header)
            {
                case "itemName":
                    result.itemName = value;
                    break;
                case "level":
                    result.level = ParserUtility.ParseIntSafe(value, "level");
                    break;
                case "price":
                    result.price = ParserUtility.ParseIntSafe(value, "price");
                    break;
                case "spritePath":
                    result.sprite = ParserUtility.ParseSpriteSafe(value, "spritePath");
                    break;
                case "maxHealth":
                    result.maxHealth = ParserUtility.ParseFloatSafe(value, "maxHealth");
                    break;
                case "attack":
                    result.attack = ParserUtility.ParseFloatSafe(value, "attack");
                    break;
                default:
                    Debug.LogWarning($"[ItemConfigParser] Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

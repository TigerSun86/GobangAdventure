using System.Collections.Generic;
using UnityEngine;

public class EnemyConfigParser : ICsvRowParser<EnemyConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "enemyId","weapon","aiStrategy"
    };

    private bool validated = false;
    private WeaponConfigDb weaponConfigDb;

    public EnemyConfigParser(WeaponConfigDb weaponConfigDb)
    {
        this.weaponConfigDb = weaponConfigDb;
    }

    public EnemyConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        EnemyConfig result = new EnemyConfig();
        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string value = values[i].Trim();

            switch (header)
            {
                case "enemyId":
                    result.enemyId = value;
                    break;
                case "weapon":
                    result.weaponConfig = this.weaponConfigDb.Get(value);
                    break;
                case "aiStrategy":
                    result.aiStrategy = ParserUtility.ParseEnum<AiStrategy>(value, ignoreCase: true);
                    break;
                default:
                    Debug.LogWarning($"Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

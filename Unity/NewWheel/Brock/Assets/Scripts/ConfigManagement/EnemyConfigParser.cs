using System.Collections.Generic;
using UnityEngine;

public class EnemyConfigParser : ICsvRowParser<RawEnemyConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "waveId","enemyId","positionX","positionY","aiStrategy","weapon"
    };

    private bool validated = false;
    private WeaponConfigDb weaponConfigDb;

    public EnemyConfigParser(WeaponConfigDb weaponConfigDb)
    {
        this.weaponConfigDb = weaponConfigDb;
    }

    public RawEnemyConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        RawEnemyConfig result = new RawEnemyConfig();
        result.enemyInFleetConfig = new EnemyInFleetConfig();
        result.enemyInFleetConfig.enemyConfig = ScriptableObject.CreateInstance<EnemyConfig>();
        result.enemyInFleetConfig.positionInFleet = Vector2.zero;

        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i].Trim();
            string value = values[i].Trim();

            switch (header)
            {
                case "waveId":
                    result.waveId = ParserUtility.ParseIntSafe(value, "waveId");
                    break;
                case "enemyId":
                    result.enemyId = ParserUtility.ParseIntSafe(value, "enemyId");
                    break;
                case "positionX":
                    result.enemyInFleetConfig.positionInFleet.x = ParserUtility.ParseFloatSafe(value, "positionX");
                    break;
                case "positionY":
                    result.enemyInFleetConfig.positionInFleet.y = ParserUtility.ParseFloatSafe(value, "positionY");
                    break;
                case "aiStrategy":
                    result.enemyInFleetConfig.enemyConfig.aiStrategy = ParserUtility.ParseEnum<AiStrategy>(value, ignoreCase: true);
                    break;
                case "weapon":
                    result.enemyInFleetConfig.enemyConfig.weaponConfig2 = this.weaponConfigDb.Get(value);
                    break;
                default:
                    Debug.LogWarning($"Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

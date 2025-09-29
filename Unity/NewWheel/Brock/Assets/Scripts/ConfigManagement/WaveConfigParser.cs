using System.Collections.Generic;
using UnityEngine;

public class WaveConfigParser : ICsvRowParser<EnemyInWaveConfig>
{
    private static readonly HashSet<string> expectedHeaders = new HashSet<string>()
    {
        "waveId","enemyId","spawnPoint","positionX","positionY","spawnDelay","spawnInterval","isBoss"
    };

    private bool validated = false;

    private EnemyConfigDb enemyConfigDb;

    public WaveConfigParser(EnemyConfigDb enemyConfigDb)
    {
        this.enemyConfigDb = enemyConfigDb;
    }

    public EnemyInWaveConfig ParseRow(string[] values, string[] headers)
    {
        if (!validated)
        {
            ParserUtility.ValidateHeaders(expectedHeaders, headers);
            validated = true;
        }

        EnemyInWaveConfig result = new EnemyInWaveConfig();
        result.positionInFleet = Vector2.zero;

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
                    result.enemyConfig = this.enemyConfigDb.Get(value);
                    break;
                case "spawnPoint":
                    result.spawnPoint = ParserUtility.ParseEnum<SpawnPoint>(value, ignoreCase: true);
                    break;
                case "positionX":
                    result.positionInFleet.x = ParserUtility.ParseFloatSafe(value, "positionX");
                    break;
                case "positionY":
                    result.positionInFleet.y = ParserUtility.ParseFloatSafe(value, "positionY");
                    break;
                case "spawnDelay":
                    result.spawnDelay = ParserUtility.ParseFloatSafe(value, "spawnDelay");
                    break;
                case "spawnInterval":
                    result.spawnInterval = ParserUtility.ParseFloatSafe(value, "spawnInterval");
                    if (result.spawnInterval < 0.5f)
                    {
                        Debug.LogWarning($"Invalid spawn interval: {result.spawnInterval}. Setting to default value of 1.");
                        result.spawnInterval = 1f;
                    }
                    break;
                case "isBoss":
                    result.isBoss = ParserUtility.ParseBoolSafe(value, "isBoss");
                    break;
                default:
                    Debug.LogWarning($"Unrecognized header '{header}' in CSV");
                    break;
            }
        }

        return result;
    }
}

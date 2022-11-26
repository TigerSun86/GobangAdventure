using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    [SerializeField] Vector2RuntimeSet spawnPositions;

    [SerializeField] float spawnChance;

    [SerializeField] float spawnInterval;

    [SerializeField] IntVariable spawnWaveNumber;

    [SerializeField] IntVariable normalEnemyMaxHealth;

    float timer;

    private void FixedUpdate()
    {
        timer -= Time.fixedDeltaTime;
        if (timer < 0f)
        {
            SpawnEnemies();
            timer = spawnInterval;
        }
    }

    private void SpawnEnemies()
    {
        spawnWaveNumber.ApplyChange(1);
        Debug.Log("Wave " + spawnWaveNumber.value);
        if (spawnWaveNumber.value % 5 == 0)
        {
            normalEnemyMaxHealth.value += 5;
            Debug.Log("Health " + normalEnemyMaxHealth.value);
        }

        int index = Random.Range(0, spawnPositions.Items.Count);
        foreach (Vector2 position in spawnPositions.Items)
        {
            if (Random.value < spawnChance)
            {
                GameObject enemyObject = Instantiate(enemy);
                enemyObject.transform.position = position;
                Level playerLevel = Manager.instance.PlayerLevel;
                enemyObject.GetComponent<Death>().died.AddListener(playerLevel.ExtractExperience);
            }
        }

    }
}

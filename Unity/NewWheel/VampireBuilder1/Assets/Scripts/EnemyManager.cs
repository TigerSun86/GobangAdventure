using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] Vector2 spawnArea;
    [SerializeField] float spawnInterval = 1f;
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
        Vector3 position = new Vector3();
        position.x = spawnArea.x;
        position.y = Random.Range(-spawnArea.y, spawnArea.y);

        GameObject enemyObject = Instantiate(enemy);
        enemyObject.transform.position = position;
        enemyObject.GetComponent<Attack>().TargetTag = "Player";
    }
}

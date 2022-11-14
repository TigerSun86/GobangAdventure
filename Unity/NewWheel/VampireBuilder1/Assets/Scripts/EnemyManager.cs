using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnAreaX;
    [SerializeField] float spawnAreaYMin;
    [SerializeField] float spawnAreaYMax;
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
        position.x = spawnAreaX;
        position.y = Random.Range(spawnAreaYMin, spawnAreaYMax);

        GameObject enemyObject = Instantiate(enemy);
        enemyObject.transform.position = position;
        Level playerLevel = Manager.instance.PlayerLevel;
        enemyObject.GetComponent<Death>().died.AddListener(playerLevel.ExtractExperience);
    }
}

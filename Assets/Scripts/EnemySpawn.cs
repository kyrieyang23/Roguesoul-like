using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    Vector3[] spawnPoints = new Vector3[2]; 
    int randomIndex;

    public int enemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints[0] = new Vector3(6.87f,-3.36f, -2.86f);
        spawnPoints[1] = new Vector3(-6.87f,-3.36f, -2.86f);
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    private IEnumerator SpawnEnemy()
    {
        while (enemyCount < 5) {
            randomIndex = Random.Range(0,2);
            Instantiate(enemy, spawnPoints[randomIndex], Quaternion.identity);
            yield return new WaitForSeconds(5);
            enemyCount += 1;
        }
    }
}

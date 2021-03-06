using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class EnemySpawn : MonoBehaviour
{
    public GameObject enemy;
    Vector3[] spawnPoints = new Vector3[2]; 
    int randomIndex;
    public int numberOfEnemy = 5;
    public PlayerStats playerStats;
    public Image Black;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints[0] = new Vector3(-12.4f,-3.36f, 11.5f);
        spawnPoints[1] = new Vector3(12.4f,-3.36f, 11.5f);
        StartCoroutine(SpawnEnemy());
        
    }

    // Update is called once per frame
    private IEnumerator SpawnEnemy()
    {
        while (true) {
            int currentEnemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if(playerStats.isDead) {
                Debug.Log("loading");
                yield return new WaitForSeconds(2);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
                
            }
            for (int i = currentEnemyCount; i < numberOfEnemy; i++) {
                randomIndex = Random.Range(0,2);
                Instantiate(enemy, spawnPoints[randomIndex], Quaternion.identity);
                yield return new WaitForSeconds(2);
            }
            yield return 0;
        }
    }

    
}

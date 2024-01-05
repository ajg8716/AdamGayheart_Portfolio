using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    //enemy prefab for spawns
    [SerializeField]
    GameObject enemy;

    [SerializeField]
    GameObject enemy2;

    [SerializeField]
    CollisionManager collisionManager;

    protected int spawns = 1;

    // Update is called once per frame
    void Update()
    {
        //if the list of enemies is empty
        if (collisionManager.enemies.Count <= 0)
        {
            //increase the spawn count by 1
            spawns += 1;

            collisionManager.player.GetComponent<SpriteInfo>().health = 3;
            collisionManager.player.Wave += 1;
            //spawn new wave
            SpawnEnemies();
        }
    }

    /// <summary>
    /// method fills the list of enemies based on spawns int. then creates a vector for the position and sets it r
    /// </summary>
    /// <param name="enemies"></param>
    public void SpawnEnemies()
    {
        //for loop that creates number of enemies for a based on the spawns integer
        for (int i = 1; i < spawns; i++)
        {
            GameObject newEnemy;
            if (i%3 == 0)
            {
                newEnemy = Instantiate(enemy2);
            }
            else
            {
                newEnemy = Instantiate(enemy);
            }
            //add the new enemy to the list
            collisionManager.enemies.Add(newEnemy);
        }

    }


}

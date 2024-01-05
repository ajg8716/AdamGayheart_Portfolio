using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSpawn : MonoBehaviour
{
    [SerializeField]
    CollisionManager collisionManager;

    [SerializeField]
    GameObject shield;

    // start is called once 
    void Start()
    {
        //if the number of enemies within collision manager is divisible by 3
        if(collisionManager.enemies.Count == 0)
        {
            //Instantiate the shield
            GameObject newShield = Instantiate(shield);
            //add the new shield to the Collision Manager List
            collisionManager.shields.Add(newShield);

        }
    }
}

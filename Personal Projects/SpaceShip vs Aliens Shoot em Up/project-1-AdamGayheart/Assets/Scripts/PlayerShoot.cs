using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    //bullet game object
    [SerializeField]
    GameObject bullet;

    //reference to the player in the scene
    private SpriteInfo player;

    //reference to the collision manager
    [SerializeField]
    CollisionManager collisionManager;

    //bullet speed 
    protected float speed = 3f;

    private void Start()
    {
        //try to find the player in the scene 
        player = collisionManager.player;
        
        //if can't find the player
        if(player == null)
        {
            //player was not found
            Debug.LogError("Player not found in the scene.");
        }

    }

    // Update is called once per frame
    void Update()
    {
        //if the left mouse button is clicked
        if (InputController.isLeftMouseClicked() ||  InputController.isEnterClicked())
        {
            //instantiate the bullet gameobject and set to new object
            GameObject newBullet = Instantiate(bullet);
            newBullet.transform.position = player.transform.position;
            newBullet.transform.rotation = player.transform.rotation;

            //add the new bullet to the list
            collisionManager.bullets.Add(newBullet);
        }
    }
}

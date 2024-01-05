using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    //bullet game object
    [SerializeField]
    GameObject bullet;

    //serialized fields to check what sprite is being used
    [SerializeField]
    Sprite enemy1;

    [SerializeField] 
    Sprite enemy2;

    [SerializeField]
    CollisionManager collisionManager; 

    //distance of camera to game window
    float camDistance = 10.0f;

    //screen constraints
    float leftConstraint = 0.0f;
    float rightConstraint = 0.0f;
    float topConstraint = 0.0f;
    float bottomConstraint = 0.0f;

    //frame delay 
    //number of frames to delay by
    [SerializeField]
    protected int delayFrames;
    [SerializeField]
    protected int frameCounter = 0;
    [SerializeField]
    protected bool isDelaying = false;

    private void Start()
    {
        //set an initial delay for shooting
        FrameCounter();

        //screen constraints left and right edges
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;

        //screen constraints top and botom
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;

        FrameCounter();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < collisionManager.enemies.Count; i++)
        {   
            if (frameCounter >= delayFrames)
            {
                //set is Delaying to false
                isDelaying = false;
                //set the counter to 0
                frameCounter = 0;
            }

            //if the delay is over
            if (isDelaying == false)
            {
                if (collisionManager.enemies[i].GetComponent<SpriteRenderer>().sprite == enemy1)
                {
                    //instantiate the bullet gameobject and set to new object
                    GameObject newBullet = Instantiate(bullet);
                    newBullet.transform.position = collisionManager.enemies[i].transform.position;

                    //create a vector3 of rotation of the bullet to look at the player
                    Vector3 rotateToPlayer = collisionManager.player.transform.position - collisionManager.enemies[i].transform.position;
                    //change rotation to look at the player
                    newBullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, rotateToPlayer);
                    //add the new bullet to the list
                    collisionManager.lasers.Add(newBullet);
                }
                else if (collisionManager.enemies[i].GetComponent<SpriteRenderer>().sprite == enemy2)
                {
                    GameObject[] lasers = new GameObject[4];

                    for(int j = 0; j < lasers.Length; j++)
                    {
                        //instantiate a new bullet
                        lasers[j] = Instantiate(bullet);
                        //set the position to the enemy
                        lasers[j].transform.position = collisionManager.enemies[i].transform.position;
                        //make rotation to be j time 45 degrees
                        lasers[j].transform.rotation = Quaternion.Euler( new Vector3(0, 0, j * 90f));
                        //add the new laser to the collision manager list
                        collisionManager.lasers.Add(lasers[j]);
                    }
                }
                //set frame counter
                FrameCounter();
            }
        }

        //increase the frame counter
        frameCounter++;
    }

    /// <summary>
    /// method created to set a random frame counter that will determine how fast the enemy will shoot
    /// </summary>
    private void FrameCounter()
    {
        //determine the amount of frames that will be initially delayed
        delayFrames = Random.Range(500, 1000);

        //set delay as true
        isDelaying = true;
    }
}

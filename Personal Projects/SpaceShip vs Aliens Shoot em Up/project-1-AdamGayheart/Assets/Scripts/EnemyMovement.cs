using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //float for speed
    [SerializeField]
    float speed;

    //vector facing +x direction]
    Vector3 direction;

    //velocity vector initially zero
    Vector3 velocity = Vector3.zero;

    //enemyPos
    Vector3 enemyPos;

    //distance of camera to game window
    float camDistance = 10.0f;

    //screen constraints
    float leftConstraint = 0.0f;
    float rightConstraint = 0.0f;
    float topConstraint = 0.0f;
    float bottomConstraint = 0.0f;

    //frame delay 
    //number of frames to delay by
    protected int delayFrames;
    protected int frameCounter = 0;
    protected bool isDelaying = false;

    private void Start()
    {
        //screen constraints left and right edges
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;

        //screen constraints top and botom
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;

        //determine the amount of frames that will be initially delayed
        delayFrames = Random.Range(180, 240);

        //random x
        enemyPos.x = Random.Range(-4f, 4f);
        //random y
        enemyPos.y = Random.Range(-5f, 5f);

        //set the position
        transform.position = enemyPos;

        SetDirection();
    }


    // Update is called once per frame
    private void Update()
    {
        if (isDelaying)
        {
            //move
            Move();
            //increase the frame counter by one
            frameCounter++;
            if (frameCounter >= delayFrames)
            {
                //set is Delaying to false
                isDelaying = false;
                //set the counter to 0
                frameCounter = 0;
            }
        }
        else
        {
            SetDirection();
        }
    }

    /// <summary>
    /// method to set the direction of where the enemy will move next
    /// </summary>
    void SetDirection()
    {
        //create random float between 0.0 and 1.0 f
        float rand = Random.value;

        //change the direction of where the enemy faces
        if(rand < .25)
        {
            direction = Vector3.right;
            //set isDelaying to true
            isDelaying = true;
        }
        else if (rand < .50)
        {
            direction = Vector3.left;
            //set isDelaying to true
            isDelaying = true;
        }
        else if(rand < .75)
        {
            direction = Vector3.up;
            //set isDelaying to true
            isDelaying = true;
        }
        else
        {
            direction = Vector3.down;
            //set isDelaying to true
            isDelaying = true;
        }       
    }

    /// <summary>
    /// method to calculate the velocity vector and apply it to the enemy
    /// </summary>
    void Move()
    {
        //check for screen wrapping
        Boundary();
        //set velocity
        velocity = direction * speed * Time.deltaTime;
        //set enemypos
        enemyPos += velocity;
        //set transform = enemy pos
        transform.position = enemyPos;
    }

    /// <summary>
    /// method to determine enemy stays withing the boundary box of the scene
    /// </summary>
    void Boundary()
    {
        //screen wrapping
        //Right Boundary
        if (enemyPos.x > rightConstraint)
        {
            direction = Vector3.left;
        }
        //Left Boundary
        if (enemyPos.x < leftConstraint)
        {
            direction = Vector3.right;
        }
        //Bottom Boundary
        if (enemyPos.y > bottomConstraint)
        {
            direction = Vector3.down;
        }
        //Top Boundary
        if (enemyPos.y < topConstraint)
        {
            direction = Vector3.up;
        }
    }
}

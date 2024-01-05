using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    //player reference
    [SerializeField]
    public SpriteInfo player;

    //List of enemy references
    public List<GameObject> enemies = new List<GameObject>();

    //list of bullet references
    public List<GameObject> bullets = new List<GameObject>();

    //list of laser references
    public List<GameObject> lasers = new List<GameObject>();

    //list of the shield references
    public List<GameObject> shields = new List<GameObject>();

    //screen constraints
    float leftConstraint = 0.0f;
    float rightConstraint = 0.0f;
    float topConstraint = 0.0f;
    float bottomConstraint = 0.0f;

    //distance of camera to game window
    float camDistance = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            Destroy(enemies[i]);
            enemies.Remove(enemies[i]);
        }

        //screen constraints left and right edges
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;

        //screen constraints top and botom
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;
    }

    // Update is called once per frame
    void Update()
    {

        //check bullets out of bounds and destroy them
        OutOfBoundsBullets();
        //check lasers out of bounds and remove them
        OutOfBoundsLasers();

        //check for entities with health <=0
        IsEntityDead();

        //check collisions between all bullets and enemies
        //loop through the bullets list
        for (int j = 0; j < bullets.Count; j++)
        {
            //loop through the enemies list
            for (int i = 0; i < enemies.Count; i++)
            {
                //check to find if the AABB for Rects check is true
                if (AABBCollides(bullets[j].GetComponent<SpriteInfo>(), enemies[i].GetComponent<SpriteInfo>()))
                {
                    //set sprite isColliding to true when is colliding
                    bullets[j].GetComponent<SpriteInfo>().IsColliding = true;
                    enemies[i].GetComponent<SpriteInfo>().IsColliding = true;
                    DecreaseHealth(enemies[i].GetComponent<SpriteInfo>());
                }
            }
            //destroy bullets that collide
            ProjectileCollide(bullets);
        }

        //player collide with shields
        //loop through shields
        for (int k = 0; k < shields.Count;)
        {
            //check to find if the AABB for Rects check is true
            if (AABBCollides(shields[k].GetComponent<SpriteInfo>(), player))
            {
                //set sprite isColliding to true when is colliding
                shields[k].GetComponent<SpriteInfo>().IsColliding = true;
                player.Shielded = true;
                DecreaseHealth(shields[k].GetComponent<SpriteInfo>());
            }

        }

        //if the player is shielded
        if (player.Shielded)
        {
            StartCoroutine(ShieldDelay());
            player.Shielded = false;
        }
        //if the player is not shielded
        else if (!player.Shielded)
        {
            //Laser collisions with player
            //loop through the lasers list
            for (int j = 0; j < lasers.Count; j++)
            {
                //check to find if the AABB for Rects check is true
                if (AABBCollides(lasers[j].GetComponent<SpriteInfo>(), player))
                {
                    //set sprite isColliding to true when is colliding
                    lasers[j].GetComponent<SpriteInfo>().IsColliding = true;
                    player.IsColliding = true;
                    DecreaseHealth(player);
                }
                //destroy the lasers that collide
                ProjectileCollide(lasers);
            }
        }
    }

    /// <summary>
    /// method that implements AABB collide check 
    /// </summary>
    /// <param name="spriteA"></param>
    /// <param name="spriteB"></param>
    /// <returns></returns>
    bool AABBCollides(SpriteInfo spriteA, SpriteInfo spriteB)
    {
        //do AABB check
        if (spriteB.RectMin.x < spriteA.RectMax.x &&
            spriteB.RectMax.x > spriteA.RectMin.x &&
            spriteB.RectMin.y < spriteA.RectMax.y &&
            spriteB.RectMax.y > spriteA.RectMin.y)
        {
            return true;
        }

        //if not colliding with AABB check return false
        return false;
    }

    /// <summary>
    /// method to check out of bounds bullets and then destroy them
    /// </summary>
    void OutOfBoundsBullets()
    {
        //loop through the bullets list
        for (int i = 0; i < bullets.Count; i++)
        {
            //if a bullet is outside of the screen window
            if (bullets[i].transform.position.x < leftConstraint ||
                bullets[i].transform.position.x > rightConstraint ||
                bullets[i].transform.position.y < topConstraint ||
                bullets[i].transform.position.y > bottomConstraint)
            {
                //it is destroyed and removed from the list
                Destroy(bullets[i]);
                bullets.Remove(bullets[i]);
            }
        }
    }

    /// <summary>
    /// method to check lasers out of bounds and then destroy them
    /// </summary>
    void OutOfBoundsLasers()
    {
        //loop through the laser list
        for (int i = 0; i < lasers.Count; i++)
        {
            //if a bullet is outside of the screen window
            if (lasers[i].transform.position.x < leftConstraint ||
                lasers[i].transform.position.x > rightConstraint ||
                lasers[i].transform.position.y < topConstraint ||
                lasers[i].transform.position.y > bottomConstraint)
            {
                //it is destroyed and removed from the list
                Destroy(lasers[i]);
                lasers.Remove(lasers[i]);
            }

        }
    }

    /// <summary>
    /// checks 
    /// </summary>
    /// <param name="collisions"></param>
    void ProjectileCollide(List<GameObject> collisions)
    {
        //loop through the list of gameobjects
        for(int i = 0;i < collisions.Count;i++)
        {
            //if it is colliding
            if (collisions[i].GetComponent<SpriteInfo>().IsColliding == true)
            {
                //destroy it
                Destroy(collisions[i]);
                //remove it from the list
                collisions.Remove(collisions[i]);
            }
        }
    }

    /// <summary>
    /// method that decreases the health of the passed through SpriteInfo 
    /// </summary>
    /// <param name="sprite"></param>
    void DecreaseHealth(SpriteInfo sprite)
    {
        //if the IsColliding bool is true
        if(sprite.IsColliding == true)
        {
            //decrease the health by 1
            sprite.health -= 1;
        }
    }

    /// <summary>
    /// checks if player and enemies havbe healths <= 0
    /// </summary>
    void IsEntityDead()
    {
        //check player health and destroy accordingly
        if(player.GetComponent<SpriteInfo>().health <= 0)
        {
            //freeze all elements
            Time.timeScale = 0;
        }
        //check enemy healths and destroy accordingly
        for(int i = 0; i < enemies.Count;i++)
        {
            //if enemies[i] health is <= 0
            if (enemies[i].GetComponent<SpriteInfo>().health <= 0)
            {
                //destroy enemies[i]
                Destroy(enemies[i]);
                //remove from the enemy list
                enemies.Remove(enemies[i]);

                //add to player score
                player.Score += 3;
            }
        }
    }

    /// <summary>
    /// shield delay using IEnumerator
    /// </summary>
    /// <returns></returns>
    IEnumerator ShieldDelay()
    {
        //creates delay for 5 seconds
        yield return new WaitForSeconds(5.0f);
    }
}

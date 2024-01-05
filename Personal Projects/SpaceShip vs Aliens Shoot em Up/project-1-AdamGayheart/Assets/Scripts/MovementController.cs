using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    //vector for object position
    Vector3 objectPos = Vector3.zero;

    //list of the sprites for the player
    [SerializeField]
    List<Sprite> sprites = new List<Sprite>();

    //float for speed given initial value so it doesn't defalt to zero
    [SerializeField]
    float speed = 1.0f;

    //vector facing +x direction]
    Vector3 direction;

    //velocity vector initially zero
    Vector3 velocity = Vector3.zero;

    //distance of camera to game window
    float camDistance = 10.0f;

    //screen constraints
    float leftConstraint = 0.0f;
    float rightConstraint = 0.0f;
    float topConstraint = 0.0f;
    float bottomConstraint = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        objectPos = transform.position;

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

        //Thrust method determines if ship will move 
        Thrust(speed);
        Rotate(direction);

        //screen wrapping
        //Right Boundary
        if (objectPos.x > rightConstraint)
        {
            objectPos.x = leftConstraint;
        }
        //Left Boundary
        if (objectPos.x < leftConstraint)
        {
            objectPos.x = rightConstraint;
        }
        //Bottom Boundary
        if (objectPos.y > bottomConstraint)
        {
            objectPos.y = topConstraint;
        }
        //Top Boundary
        if (objectPos.y < topConstraint)
        {
            objectPos.y = bottomConstraint;
        }
    }

    /// <summary>
    /// thrust method determines how the player will move. this is by hitting space so that the thrusters will engage and the ship will move forward
    /// </summary>
    /// <param name="speed"></param>
    public void Thrust(float speed)
    {
        //temporary float to represent speed to be manipulated as reference to set speed
        float newSpeed = speed;
        //check if space is being pressed
        if (InputController.isWDown())
        {
            //
            //change sprite code
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
            //

            //set the newspeed equal to the serialized
            newSpeed = speed;

            //velocity vector
            velocity = transform.up * speed * Time.deltaTime;

            //translate the object based on 
            objectPos += velocity;

            //set the object postion = to the transform
            transform.position = objectPos;
        }
        else
        {
            //
            //change sprite code
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
            //
        }
    }

    /// <summary>
    /// Rotate method determiones how the playr will rotate the ship. A will rotate counter-clockwise and D will rotate clockwise
    /// </summary>
    /// <param name="newdirection"></param>
    public void Rotate(Vector3 newdirection)
    {
        //if the new direction is not null
        if(newdirection != null)
        {
            //set the new direction to the direction
            direction = newdirection;

            //if a is being pressed in this frame
            if (InputController.isADown())
            {
                //rotate the sprite using a quaternion counter-clockwise by 35 degrees per frame
                transform.rotation = Quaternion.Euler(0, 0, .50f) * transform.rotation;
            }
            //if d is being pressed in this frame
            if (InputController.isDDown())
            {
                //rotate the sprite using a quaternion clockwise by 35 degrees per frame
                transform.rotation = Quaternion.Euler(0, 0, -.65f) * transform.rotation;
            }
        } 
    }
}

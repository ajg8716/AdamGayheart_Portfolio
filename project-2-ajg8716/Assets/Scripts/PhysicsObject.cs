using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    //vectors for object
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public Vector3 direction;
    [SerializeField]
    Vector3 velocity;

    [SerializeField]
    float radius = 0;

    //vector for aceleration
    [SerializeField]
    Vector3 acceleration = Vector3.zero;

    //mass must be defaukt 1 because you cannot divide by zero
    [SerializeField]
    float mass = 1f;

    [SerializeField]
    float maxSpeed = 10;

    //distance of camera to game window
    float camDistance = 10.0f;

    [SerializeField]
    bool hasFriction = false;

    [SerializeField]
    float frictionCoefficient;

    //sprites
    [SerializeField]
    private List<Sprite> sprites = new List<Sprite>();

    //property for sprite list
    public List<Sprite> Sprites { get { return sprites; } }


    //screen constraints
    private float leftConstraint = 0.0f;
    private float rightConstraint = 0.0f;
    private float topConstraint = 0.0f;
    private float bottomConstraint = 0.0f;

    public float Radius
    {
        get { return radius; }
    }

    public Vector3 Velocity { get { return velocity; } }

    public float MaxSpeed { get { return maxSpeed; } }

    // Start is called before the first frame update
    void Start()
    {
        //screen constraints left and right edges
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;

        //screen constraints top and botom
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;

        //set the position to where they are in the scene
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFriction)
        {
            ApplyFriction(frictionCoefficient);
        }

        // Calculate the velocity for this frame - New
        velocity += acceleration * Time.deltaTime;

        //clamp the velocity max
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        position += velocity * Time.deltaTime;

        // Grab current direction from velocity  - New
        direction = velocity.normalized;

        transform.position = position;

        // Zero out acceleration - New
        acceleration = Vector3.zero;

        transform.rotation = Quaternion.LookRotation(Vector3.back, direction);
    }

    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        ApplyForce(friction);
    }

    void CheckBounds()
    {
        if (position.x <= leftConstraint)
        {
            velocity.x *= -1;
            position.x = leftConstraint;
        }
        else if (position.x >= rightConstraint)
        {
            velocity.x *= -1;
            position.x = rightConstraint;
        }
        else if (position.y <= topConstraint)
        {
            velocity.y *= -1;
            position.y = topConstraint;
        }
        else if (position.y >= bottomConstraint)
        {
            velocity.y *= -1;
            position.y = bottomConstraint;
        }
    }
}



using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    [SerializeField]
    protected AgentManager agentsManager;

    public AgentManager AgentsManager { get { return agentsManager; } set { agentsManager = value; } }

    [SerializeField]
    public PhysicsObject myPhysicsObject;

    [SerializeField]
    float separateRange = 1.0f;

    [SerializeField]
    protected float boundsWeight = 10f;

    [SerializeField]
    protected float avoidTime = 1f;

    [SerializeField]
    protected float avoidWeight = 1f;

    protected Vector3 UltimateForce = Vector3.zero;

    [SerializeField]
    float maxForce = 10;


    //screen constraints
    float leftConstraint = 0.0f;
    float rightConstraint = 0.0f;
    float topConstraint = 0.0f;
    float bottomConstraint = 0.0f;

    float camDistance = 10.0f;

    public float avoidDist = 1f;

    protected List<Vector3> foundObstacles = new List<Vector3>();

    private bool isFlocking;
    private bool isFleeing;

    public bool IsFlocking
    {
        get { return isFlocking; }
    }

    public bool IsFleeing
    {
        get { return isFleeing; }
    }

    private void Start()
    {
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
        UltimateForce = Vector3.zero;

        CalcSteeringForces();

        //limits how large the vectr can ever be
        UltimateForce = Vector3.ClampMagnitude(UltimateForce, maxForce);

        myPhysicsObject.ApplyForce(UltimateForce);;

        // Loop through all agents and keep them within bounds
        foreach (Agent agent in agentsManager.AgentsRock)
        {
            agent.StayInBounds();
        }

        foreach (Agent agent in agentsManager.AgentsPaper)
        {
            agent.StayInBounds();
        }

        foreach (Agent agent in agentsManager.AgentsScissors)
        {
            agent.StayInBounds();
        }
    }

    protected Vector3 Separate()
    {
        Vector3 separateForce = Vector3.zero;

        if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[0])
        {
            foreach (Agent a in agentsManager.AgentsRock)
            {
                float dist = Vector3.Distance(transform.position, a.transform.position);

                if (Mathf.Epsilon < dist)
                {
                    separateForce += Flee(a.transform.position) * (separateRange / dist);
                }
            }
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[1])
        {
            foreach (Agent a in agentsManager.AgentsPaper)
            {
                float dist = Vector3.Distance(transform.position, a.transform.position);

                if (Mathf.Epsilon < dist)
                {
                    separateForce += Flee(a.transform.position) * (separateRange / dist);
                }
            }
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[2])
        {
            foreach (Agent a in agentsManager.AgentsScissors)
            {
                float dist = Vector3.Distance(transform.position, a.transform.position);

                if (Mathf.Epsilon < dist)
                {
                    separateForce += Flee(a.transform.position) * (separateRange / dist);
                }
            }
        }

        return separateForce;
    }

    protected Vector3 Cohesion()
    {
        Vector3 cohesion = Vector3.zero;

        List<Agent> agents = null;

        if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[0])
        {
            agents = agentsManager.AgentsRock;
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[1])
        {
            agents = agentsManager.AgentsPaper;
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[2])
        {
            agents = agentsManager.AgentsScissors;
        }

        if (agents != null)
        {
            cohesion = FlockManager.instance.GetCenterPoint(agents);
        }

        return Seek(cohesion);
    }

    protected Vector3 Alignment()
    {
        Vector3 desiredVelocity = Vector3.zero;

        List<Agent> agents = null; 

        if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[0])
        {
            agents = agentsManager.AgentsRock;
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[1])
        {
            agents = agentsManager.AgentsPaper;
        }
        else if (this.GetComponent<SpriteRenderer>().sprite == this.myPhysicsObject.Sprites[2])
        {
            agents = agentsManager.AgentsScissors;
        }

        if (agents != null)
        {
            desiredVelocity = myPhysicsObject.MaxSpeed * FlockManager.instance.GetSharedDirection(agents);
        }

        return desiredVelocity - myPhysicsObject.Velocity;
    }


    protected abstract void CalcSteeringForces();

    protected Vector3 Seek(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = targetPos - transform.position;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * myPhysicsObject.MaxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - myPhysicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    protected Vector3 Seek(Agent target)
    {
        return Seek(target.transform.position);
    }

    protected Vector3 Flee(Vector3 targetPos)
    {
        // Calculate desired velocity
        Vector3 desiredVelocity = transform.position - targetPos;

        // Set desired = max speed
        desiredVelocity = desiredVelocity.normalized * myPhysicsObject.MaxSpeed;

        // Calculate seek steering force
        Vector3 seekingForce = desiredVelocity - myPhysicsObject.Velocity;

        // Return seek steering force
        return seekingForce;
    }

    protected Vector3 Wander(float time, float radius)
    {
        Vector3 futurePos = CalcFuturePosition(time);

        float randAngle = Random.Range(0, Mathf.PI * 2);

        Vector3 targetPos = futurePos;
        targetPos.x += Mathf.Cos(randAngle) * radius;
        targetPos.y += Mathf.Sin(randAngle) * radius;

        //seek to the point
        return Seek(targetPos);
    }

    protected Vector3 CalcFuturePosition(float time)
    {
        return myPhysicsObject.Velocity * time + transform.position;
    }

    protected Vector3 Flee(GameObject target)
    {
        return Flee(target.transform.position);
    }

    public Vector3 StayInBounds()
    {
        if (transform.position.x <= leftConstraint ||
            transform.position.x >= rightConstraint ||
            transform.position.y <= topConstraint ||
            transform.position.y >= bottomConstraint)
        {
            return Seek(Vector3.zero);
        }

        return Vector3.zero;
    }

    protected Vector3 AvoidObstacles(float avoidTime)
    {
        Vector3 totalAvoidForce = Vector3.zero;

        //loop through the obstacles of scene manager
        foreach (Obstacle obstacle in AgentsManager.obstacles)
        {
            //create a vector from agent to obstacle
            Vector3 aToO = obstacle.transform.position - transform.position;

            //calculate the forward and right dot products
            float forwardDot = Vector3.Dot(myPhysicsObject.direction, aToO);
            float rightDot = Vector3.Dot(transform.right, aToO);


            //copied from wander
            Vector3 futurePos = CalcFuturePosition(avoidTime);

            float dist = Vector3.Distance(transform.position, futurePos) + myPhysicsObject.Radius;

            //check if forward
            if (forwardDot >= 0 && forwardDot <= avoidDist && Mathf.Abs(rightDot) <= myPhysicsObject.Radius + obstacle.radius)
            {
                //Assumes agent is looking at its movement direction
                rightDot = Vector3.Dot(transform.right, aToO);

                //check if too far left/right
                if (Mathf.Abs(rightDot) <= myPhysicsObject.Radius + obstacle.radius)
                {
                    //Add found obstacle to list
                    foundObstacles.Add(obstacle.transform.position);
                    totalAvoidForce += transform.right * -Mathf.Sign(rightDot);
                }
            }
            else
            {
                //remove the obstacle from the found list
                foundObstacles.Remove(obstacle.transform.position);
            }
        }

        return totalAvoidForce;
    }

    public void EnableFlocking()
    {
        isFlocking = true;
    }

    public void StopFlocking()
    {
        isFlocking = false;
    }
}



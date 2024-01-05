using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public Agent agentPrefab;

    public List<Obstacle> obstacles;

    protected List<Agent> agentsScissors = new List<Agent>();
    protected List<Agent> agentsRock = new List<Agent>();
    protected List<Agent> agentsPaper = new List<Agent>();

    //value for how close agents of different types will need to be together to determine behavior switches
    [SerializeField]
    protected float proxiThresh = 1.0f;


    public List<Agent> AgentsScissors
    {
        get { return agentsScissors; }
    }
    public List<Agent> AgentsRock
    {
        get { return agentsRock; }
    }
    public List<Agent> AgentsPaper
    {
        get { return agentsPaper; }
    }

    private const int initialAgents = 3;

    //distance of camera to game window
    float camDistance = 10.0f;

    //screen constraints
    private float leftConstraint = 0.0f;
    private float rightConstraint = 0.0f;
    private float topConstraint = 0.0f;
    private float bottomConstraint = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //screen constraints left and right edges
        leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).x;
        rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, camDistance)).x;

        //screen constraints top and botom
        topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, camDistance)).y;
        bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, camDistance)).y;

        for (int i = 0; i < initialAgents; i++)
        {
            //create all the initial agents
            for (int j = 0; j < initialAgents; j++)
            {
                Agent newAgent = Instantiate(agentPrefab);
                newAgent.GetComponent<Wander>().AgentsManager = this;
                newAgent.GetComponent<Seek>().AgentsManager = this;
                newAgent.GetComponent<Flee>().AgentsManager = this;

                //determine which list the agent will be added to
                switch (i)
                {
                    //rock
                    case 0:
                        newAgent.GetComponent<SpriteRenderer>().sprite = newAgent.myPhysicsObject.Sprites[0];
                        //Gaussian random for spawn
                        newAgent.myPhysicsObject.transform.position = new Vector3(Gaussian(leftConstraint + 2f, .5f), Gaussian(topConstraint + 1f, 1f), 0f);
                        agentsRock.Add(newAgent);
                        break;
                    //paper
                    case 1:
                        newAgent.GetComponent<SpriteRenderer>().sprite = newAgent.myPhysicsObject.Sprites[1];
                        //Gaussian random for spawn
                        newAgent.myPhysicsObject.transform.position = new Vector3(Gaussian(leftConstraint + 2f, .5f), Gaussian(bottomConstraint - 1f, 1f), 0f);
                        agentsPaper.Add(newAgent);
                        break;
                    //scissors
                    case 2:
                        newAgent.GetComponent<SpriteRenderer>().sprite = newAgent.myPhysicsObject.Sprites[2];
                        //Gaussian random for spawn
                        newAgent.myPhysicsObject.transform.position = new Vector3(Gaussian(rightConstraint - 2f, .5f), Gaussian(topConstraint + 1f, 1f), 0f);
                        agentsScissors.Add(newAgent);
                        break;
                }
            }
        }

    }

    private void Update()
    {
        //check for newly created agents
        InputForAgentCreator(); 

        //check for prey agents in proximity
        CheckAgentProximity(AgentsRock, AgentsScissors);
        CheckAgentProximity(AgentsScissors, AgentsPaper);
        CheckAgentProximity(AgentsPaper, AgentsRock);

        //check for agent with prey 
        CheckAgentCollisions(AgentsRock, AgentsScissors, agentPrefab.myPhysicsObject.Sprites[0]);
        CheckAgentCollisions(AgentsPaper, AgentsRock, agentPrefab.myPhysicsObject.Sprites[1]);
        CheckAgentCollisions(AgentsScissors, AgentsPaper, agentPrefab.myPhysicsObject.Sprites[2]);

        CheckAgentCollisions(AgentsRock, obstacles);
        CheckAgentCollisions(AgentsPaper, obstacles);
        CheckAgentCollisions(AgentsScissors, obstacles);
    }

    /// <summary>
    /// this Gaussian method is taken from the slides and will help in setting spawn positions of the initial agents
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="stdDev"></param>
    /// <returns></returns>
    float Gaussian(float mean, float stdDev)
    {
        float val1 = Random.Range(0f, 1f);
        float val2 = Random.Range(0f, 1f);
        float gaussValue =
                 Mathf.Sqrt(-2.0f * Mathf.Log(val1)) *
                 Mathf.Sin(2.0f * Mathf.PI * val2);
        return mean + stdDev * gaussValue;
    }

    /// <summary>
    /// Method checks for proximity between agents in a list and then determines behavior 
    /// - list 1 seeks
    /// - list 2 flees
    /// </summary>
    /// <param name="agentList1"></param>
    /// <param name="agentList2"></param>
    private void CheckAgentProximity(List<Agent> agentList1, List<Agent> agentList2)
    {
        foreach (Agent agent1 in agentList1)
        {
            foreach (Agent agent2 in agentList2)
            {
                float distance = Vector3.Distance(agent1.transform.position, agent2.transform.position);
                // Check proximity between agents from two different lists
                if (distance < proxiThresh)
                {
                    // Disable wanders
                    agent1.GetComponent<Wander>().enabled = false;
                    agent2.GetComponent<Wander>().enabled = false;

                    // Enable the respective behaviors (Seek and Flee)
                    agent1.GetComponent<Seek>().enabled = true; // Agent 1 seeks Agent 2
                    // Set the target of Agent 1 to Agent 2
                    agent1.GetComponent<Seek>().Target = agent2;

                    agent2.GetComponent<Flee>().enabled = true; // Agent 2 flees from Agent 1
                    // Set the target of Agent 2 to Agent 1
                    agent2.GetComponent<Flee>().Target = agent1;
                }
                else if (distance > proxiThresh)
                {
                    // Enable wanders
                    agent1.GetComponent<Wander>().enabled = true;
                    agent2.GetComponent<Wander>().enabled = true;

                    // Disable the respective behaviors (Flee and Seek)
                    agent1.GetComponent<Seek>().enabled = false; // Agent 1 stops seeking Agent 2
                    // Set the target of Agent 1 to null
                    agent1.GetComponent<Seek>().Target = null;

                    agent2.GetComponent<Flee>().enabled = false; // Agent 2 stops fleeing from Agent 1
                    // Set the target of Agent 2 to null
                    agent2.GetComponent<Flee>().Target = null;
                }
            }
        }
    }

    /// <summary>
    /// checks for collide between two objects within the input lists and then changes the agent2 to an agent1 and adds it to the agent1 list
    /// </summary>
    /// <param name="agentList1"></param>
    /// <param name="agentList2"></param>
    /// <param name="newSprite"></param>
    private void CheckAgentCollisions(List<Agent> agentList1, List<Agent> agentList2, Sprite newSprite)
    {
        //loop through agent list1
        foreach (Agent agent1 in agentList1)
        {
            //loop through agent list2
            foreach (Agent agent2 in agentList2)
            {
                if (Vector3.Distance(agent1.transform.position, agent2.transform.position) < proxiThresh)
                {
                    // Collide agents
                    if (Collide(agent1.myPhysicsObject, agent2.myPhysicsObject))
                    {
                        // Change the sprite of agent2
                        agent2.GetComponent<SpriteRenderer>().sprite = newSprite;

                        // Remove agent2 from agentList2 and add it to agentList1
                        agentList2.Remove(agent2);
                        agentList1.Add(agent2);
                    }
                }
            }
        }
    }

    private void CheckAgentCollisions(List<Agent> agentList, List<Obstacle> obstacleList)
    {
        //loop through agent list
        foreach (Agent agent in agentList)
        {
            //loop through obstacle list
            foreach (Obstacle obstacle in obstacleList)
            {
                if (Vector3.Distance(agent.transform.position, obstacle.transform.position) < proxiThresh)
                {
                    if (Collide(agent.myPhysicsObject, obstacle))
                    {
                        agentList.Remove(agent);
                        Destroy(agent.gameObject);
                    }
                }
            }
        }
    }

    // Method to check collision between two physics objects
    private bool Collide(PhysicsObject object1, PhysicsObject object2)
    {
        if (Mathf.Pow((object1.transform.position.x - object2.transform.position.x), 2) + Mathf.Pow((object1.transform.position.y - object2.transform.position.y), 2) < Mathf.Pow(object1.Radius, 2) + Mathf.Pow(object2.Radius, 2))
        {
            return true;
        }
        return false;
    }
    private bool Collide(PhysicsObject object1, Obstacle obstacle)
    {
        if (Mathf.Pow((object1.transform.position.x - obstacle.transform.position.x), 2) + Mathf.Pow((object1.transform.position.y - obstacle.transform.position.y), 2) < Mathf.Pow(object1.Radius, 2) + Mathf.Pow(obstacle.radius, 2))
        {
            return true;
        }
        return false;
    }

    private void InputForAgentCreator()
    {
        //mouse position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        mousePos.z = 0f;

        //create new agents based on key presses
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateAgent(mousePos, agentsRock, agentPrefab.myPhysicsObject.Sprites[0]);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            CreateAgent(mousePos, agentsScissors, agentPrefab.myPhysicsObject.Sprites[2]);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            CreateAgent(mousePos, agentsPaper, agentPrefab.myPhysicsObject.Sprites[1]);
        }
    }

    private void CreateAgent(Vector3 mousePos, List<Agent> agentList, Sprite sprite)
    {
        //instantiate from the prefab
        Agent newAgent = Instantiate(agentPrefab);
        //set references to the angentManager in each script
        newAgent.GetComponent<Wander>().AgentsManager = this;
        newAgent.GetComponent<Seek>().AgentsManager = this;
        newAgent.GetComponent<Flee>().AgentsManager = this;

        //set the sprite
        newAgent.GetComponent<SpriteRenderer>().sprite = sprite;
        //set the position to the mouse position
        newAgent.myPhysicsObject.transform.position = mousePos;
        // Add new agent to the respective agent list
        agentList.Add(newAgent);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class FlockManager : MonoBehaviour
{
    [SerializeField]
    private AgentManager agentManager;

    public static FlockManager instance;

    protected float avoidTime = 1.0f;

    [SerializeField]
    protected float proxiThresh = 2.0f;

    //list of the found obstacles
    private List<Vector3> rockObstacles = new List<Vector3>();
    private List<Vector3> paperObstacles = new List<Vector3>();
    private List<Vector3> scissorObstacles = new List<Vector3>();

    private List<Vector3> centerPoints = new List<Vector3>();
    private List<Vector3> sharedDirections = new List<Vector3>();

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


    // Update is called once per frame
    void Update()
    {
        centerPoints.Clear();
        sharedDirections.Clear();

        //calculate flocks 
        FlockTogether(agentManager.AgentsRock);
        FlockTogether(agentManager.AgentsPaper);
        FlockTogether(agentManager.AgentsScissors);
    }

    public Vector3 GetCenterPoint(List<Agent> Agents)
    {
        Vector3 sumVector = Vector3.zero;
        if (agentManager != null)
        {
            foreach (Agent agent in Agents)
            {
                sumVector += agent.transform.position;
                Debug.Log(sumVector);
            }
            return sumVector / Agents.Count;
        }
        else
        {
            Debug.LogError("AgentManager is not assigned to FlockManager");
            return Vector3.zero;
        }
    }

    public Vector3 GetSharedDirection(List<Agent> Agents)
    {
        Vector3 sumDirection = Vector3.zero;
        if (agentManager != null)
        {
            foreach (Agent agent in Agents)
            {
                sumDirection += agent.transform.up;
            }
            return sumDirection.normalized;
        }
        else
        {
            Debug.LogError("AgentManager is not assigned to FlockManager");
            return Vector3.zero;
        }
    }

    /// <summary>
    /// this method will loop through a list of agent types to see if they are close to eachother 
    /// </summary>
    /// <param name="agents"></param>
    public void FlockTogether(List<Agent> agents)
    {
        //temprorary list that will store all objects that are close together for flocking
        List<Agent> foundAgents = new List<Agent>();    

        //loop through the list 
        for (int i = 0; i < agents.Count; i++)
        {
            //loop through again
            for (int j = 0; j < agents.Count; j++)
            {
                float distance = Vector3.Distance(agents[i].transform.position, agents[j].transform.position);
                //conditional to check if two objects are close to eachother
                if (distance < proxiThresh)
                {
                    if (agents[i].GetComponent<Wander>().enabled && agents[j].GetComponent<Wander>().enabled)
                    {
                        // Check if agents are not already in the foundAgents list before adding
                        if (!foundAgents.Contains(agents[i]))
                        {
                            foundAgents.Add(agents[i]);
                            agents[i].EnableFlocking();
                        }
                        if (!foundAgents.Contains(agents[j]))
                        {
                            foundAgents.Add(agents[j]);
                            agents[j].EnableFlocking();
                        }
                    }
                }
                else if(distance > proxiThresh)
                {
                    foundAgents.Remove(agents[i]);
                    foundAgents.Remove(agents[j]);
                }
            }
        }

        //get the center point and shared direction of this list of found obstacles
        Vector3 centerPoint = GetCenterPoint(foundAgents);
        //add the new centerpoint to the list of centerpoints
        centerPoints.Add(centerPoint);

        Vector3 sharedDirection = GetSharedDirection(foundAgents);
        //add the shared direction to the list of shared directions
        sharedDirections.Add(sharedDirection);

    }

    private void OnDrawGizmos()
    {
        // Draw gizmos using centerPoint and sharedDirection
        for (int i = 0; i < centerPoints.Count; i++)
        {
            Gizmos.color = Color.red; // Get a color for each flock type
            Gizmos.DrawSphere(centerPoints[i], .05f);
            Gizmos.DrawLine(centerPoints[i], centerPoints[i] + sharedDirections[i]);
        }
    }
}


   

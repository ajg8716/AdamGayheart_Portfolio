using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : Agent
{
    [SerializeField]
    private Agent target;

    public Agent Target
    {
        get { return target; }
        set { target = value; }
    }

    //no start or update as it is a child of agent and is implemented in the Agent update loop
    protected override void CalcSteeringForces()
    {

        UltimateForce += Seek(target);

        UltimateForce += StayInBounds() * boundsWeight;

        UltimateForce += AvoidObstacles(avoidTime) * avoidWeight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position, transform.position + myPhysicsObject.Velocity);
    }
}

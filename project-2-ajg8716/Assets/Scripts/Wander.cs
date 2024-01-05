using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : Agent
{
    [SerializeField]
    protected float wanderRadius = 1f;

    [SerializeField]
    protected float wanderTime = 1f;

    [SerializeField]
    protected float wanderWeight = 1f;

    protected override void CalcSteeringForces()
    {
        UltimateForce += Wander(wanderTime, wanderRadius) * wanderWeight;

        UltimateForce += StayInBounds() * boundsWeight;

        UltimateForce += AvoidObstacles(avoidTime) * avoidWeight;

        UltimateForce += Cohesion();

        UltimateForce += Separate();

        UltimateForce += Alignment();
    }


    private void OnDrawGizmos()
    {
        //
        //  Draw safe space box
        Vector3 futurePos = CalcFuturePosition(avoidTime);

        Vector3 boxSize = new Vector3(myPhysicsObject.Radius * 2f,
            avoidDist
            , myPhysicsObject.Radius * 2f);

        Vector3 boxCenter = Vector3.zero;
        boxCenter.y += avoidDist / 2f;

        Gizmos.color = Color.green;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;


        //
        //  Draw lines to found obstacles
        //
        Gizmos.color = Color.red;

        foreach (Vector3 pos in foundObstacles)
        {
            Gizmos.DrawLine(transform.position, pos);
        }
    }
}


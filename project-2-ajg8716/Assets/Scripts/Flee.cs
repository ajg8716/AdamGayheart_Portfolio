using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Agent
{
    [SerializeField]
    private Agent target;

    public Agent Target
    {
        get { return target; }
        set { target = value; }
    }

    Vector3 fleeForce = Vector3.zero;

    //distance of camera to game window
    float camDistance = 10.0f;

    protected override void CalcSteeringForces()
    {
        UltimateForce += Flee(target.transform.position);

        UltimateForce += StayInBounds() * boundsWeight;

        UltimateForce += AvoidObstacles(avoidTime) * avoidWeight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawLine(transform.position, transform.position + myPhysicsObject.Velocity);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + fleeForce);
    }

    bool Collide(PhysicsObject monsterA, PhysicsObject monsterB)
    {
        if (Mathf.Pow((monsterA.transform.position.x - monsterB.transform.position.x), 2) + Mathf.Pow((monsterA.transform.position.y - monsterB.transform.position.y), 2) < Mathf.Pow(monsterA.Radius, 2) + Mathf.Pow(monsterB.Radius, 2))
        {
            return true;
        }
        return false;
    }
}

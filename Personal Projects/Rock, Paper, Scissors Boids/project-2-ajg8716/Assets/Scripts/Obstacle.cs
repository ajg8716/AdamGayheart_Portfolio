using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float radius = 1f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

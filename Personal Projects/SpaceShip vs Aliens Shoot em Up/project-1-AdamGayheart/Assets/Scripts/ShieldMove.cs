using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMove : MonoBehaviour
{
    //enemyPos
    Vector3 shieldPos;

    private void Start()
    {
        //random x
        shieldPos.x = Random.Range(-4f, 4f);
        //random y
        shieldPos.y = Random.Range(-4f, 4f);

        //set the position
        transform.position = shieldPos;
    }
}


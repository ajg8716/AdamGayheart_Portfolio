using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuletMove : MonoBehaviour
{
    //reference to the player in the scene
    private GameObject player;

    //direction of the bullet
    Vector3 direction;

    //velocity
    Vector3 velocity = Vector3.zero;

    //vector for bullet position
    Vector3 bulletPos = Vector3.zero;

    //speed
    [SerializeField]
    private float speed = 4f;

    // Start is called before the first frame update
    void Start()
    {
        //set te direction
        direction = transform.up;
        //set the bullet position to the transform
        bulletPos = transform.position;
        //set the z to 1 so that the bullet appears behind the player
        bulletPos.z = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        //set the velocity
        velocity = direction * speed * Time.deltaTime;
        //set the transform to the velocity
        bulletPos += velocity;
        //set the transform to the bulletPos
        transform.position = bulletPos;
    }
}

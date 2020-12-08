using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeBehaviour : MonoBehaviour
{
    public GameManager.RigidBody rigidBody = new GameManager.RigidBody();

    // Start is called before the first frame update
    void Start()
    {
        GameManager.collisionManager.Cubes.Add(this);
        rigidBody.velocity = new Vector3(0, 0, 0);
        rigidBody.acceleration = new Vector3(0, 0, 0);
        rigidBody.mass = 1;
        rigidBody.restitution = 0.8f;
        rigidBody.friction = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity += rigidBody.acceleration * Time.deltaTime;
        transform.position += rigidBody.velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));*/
    }
}

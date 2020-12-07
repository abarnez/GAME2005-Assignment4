using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeBehaviour : MonoBehaviour
{
    private GameObject collisionManagerObject;
    private CollisionManager collisionManager;

    public struct RigidBody
    {
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Mass;
        public float Restitution;
        public float Friction;
    }

    public RigidBody rigidBody = new RigidBody();

    // Start is called before the first frame update
    void Start()
    {
        collisionManagerObject = GameObject.FindWithTag("CollisionManager");
        collisionManager = collisionManagerObject.GetComponent<CollisionManager>();
        collisionManager.Cubes.Add(this);
        rigidBody.Velocity = new Vector3(0, 0, 0);
        rigidBody.Acceleration = new Vector3(0, 0, 0);
        rigidBody.Mass = 3;
        rigidBody.Restitution = 0.5f;
        rigidBody.Friction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.Velocity += rigidBody.Acceleration * Time.deltaTime;
        transform.position += rigidBody.Velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private Vector3 forwardDirection;
    private float forwardVelocity;

    private GameObject collisionManagerObject;
    private CollisionManager collisionManager;
    public struct RigidBody
    {
        public Vector3 Velocity;
        public Vector3 Acceleration;
        public float Mass;
    }

    public RigidBody rigidBody = new RigidBody();

    // Start is called before the first frame update
    void Start()
    {
        forwardDirection = GameObject.FindWithTag("Player").transform.forward;

        collisionManagerObject = GameObject.FindWithTag("CollisionManager");
        collisionManager = collisionManagerObject.GetComponent<CollisionManager>();
        collisionManager.Spheres.Add(this);

        forwardVelocity = 3;
        //Debug.Log("Direction = " + forwardDirection);
        
        rigidBody.Velocity = new Vector3(0, 0, 0);
        rigidBody.Acceleration = new Vector3(0, 0, 0);
        rigidBody.Mass = 0;

        rigidBody.Velocity = forwardDirection * forwardVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.Velocity += rigidBody.Acceleration * Time.deltaTime;
        transform.position += rigidBody.Velocity * Time.deltaTime;
    }
}

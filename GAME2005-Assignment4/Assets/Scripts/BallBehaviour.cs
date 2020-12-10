using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private Vector3 forwardDirection;
    private float forwardVelocity;

    public GameManager.RigidBody rigidBody = new GameManager.RigidBody();

    // Start is called before the first frame update
    void Start()
    {
        CollisionManager.Instance.Spheres.Add(this);

        forwardDirection = GameObject.FindWithTag("Player").transform.forward;
        forwardVelocity = 12;
        rigidBody.velocity = new Vector3(0, 0, 0);
        rigidBody.acceleration = new Vector3(0, -0.5f, 0);
        rigidBody.mass = 1;
        rigidBody.restitution = 1;
        rigidBody.friction = 0.5f;
        rigidBody.anchored = false;

        rigidBody.velocity = forwardDirection * forwardVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity += rigidBody.acceleration * Time.deltaTime;
        transform.position += rigidBody.velocity * Time.deltaTime;
    }
}

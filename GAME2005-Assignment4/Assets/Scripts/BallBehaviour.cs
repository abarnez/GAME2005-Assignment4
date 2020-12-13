using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private Vector3 forwardDirection;
    public float forwardSpeed;
    public float gravity;

    public RigidBody rigidBody = new RigidBody();
    public Contact contacts = new Contact();

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        CollisionManager.Instance.Spheres.Add(this);

        rigidBody.velocity = new Vector3(0, 0, 0);
        rigidBody.acceleration = new Vector3(0, gravity, 0);
        rigidBody.mass = 1;
        rigidBody.restitution = 0.8f;
        rigidBody.friction = 0.8f;
        rigidBody.anchored = false;
        rigidBody.radius = transform.localScale.x * 0.5f;

        rigidBody.velocity = transform.forward * forwardSpeed;

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity += rigidBody.acceleration * Time.deltaTime;
        transform.position += rigidBody.velocity * Time.deltaTime;
        if(rigidBody.velocity.magnitude <= 0.1f)
        {
            RemoveObject();
        }
        else if(Vector3.Distance(transform.position, startPosition) >= 30)
        {
            RemoveObject();
        }
    }

    void RemoveObject()
    {
        CollisionManager.Instance.Spheres.Remove(this);
        GameObject.Destroy(this.gameObject);
    }
}

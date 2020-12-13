using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class CubeBehaviour : MonoBehaviour
{
    public RigidBody rigidBody = new RigidBody();

    public float forwardSpeed = 1;

    public bool anchored;
    public float gravity;

    public Contact contacts = new Contact();

    // Start is called before the first frame update
    public void Init()
    {
        rigidBody.velocity = transform.forward * forwardSpeed;
    }

    void Start()
    {
        CollisionManager.Instance.Cubes.Add(this);
        rigidBody.velocity = new Vector3(0, 0, 0);
        rigidBody.acceleration = new Vector3(0, gravity, 0);
        rigidBody.mass = 5;
        rigidBody.restitution = 0.8f;
        rigidBody.friction = 0.6f;
        rigidBody.anchored = anchored;
        //textTransform = floatingText.GetComponent<RectTransform>();
    }
    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity += rigidBody.acceleration * Time.deltaTime;
        transform.position += rigidBody.velocity * Time.deltaTime;

        //floatingText.text = rigidBody.velocity.magnitude.ToString("F2") + " m/s\n" + rigidBody.mass + " kg\nFriction " + rigidBody.friction;
        //textTransform.transform.rotation = Camera.main.transform.rotation;
    }
}

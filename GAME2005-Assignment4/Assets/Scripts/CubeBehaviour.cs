using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeBehaviour : MonoBehaviour
{
    public GameManager.RigidBody rigidBody = new GameManager.RigidBody();

    public bool Anchored;

    // Start is called before the first frame update
    void Start()
    {
        CollisionManager.Instance.Cubes.Add(this);
        rigidBody.velocity = new Vector3(0, !Anchored? -0.5f : 0, 0);
        rigidBody.acceleration = new Vector3(0, 0, 0);
        rigidBody.mass = 5;
        rigidBody.restitution = 0.2f;
        rigidBody.friction = 0.5f;
        rigidBody.anchored = Anchored;
    }

    // Update is called once per frame
    void Update()
    {
        rigidBody.velocity += rigidBody.acceleration * Time.deltaTime;
        transform.position += rigidBody.velocity * Time.deltaTime;
    }
}

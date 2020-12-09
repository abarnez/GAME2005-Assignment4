using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public struct RigidBody
    {
        public Vector3 velocity;
        public Vector3 acceleration;
        public float mass;
        public float restitution;
        public float friction;
        public bool anchored;
    }

    public static CollisionManager collisionManager;

    // Start is called before the first frame update
    void Start()
    {
        collisionManager = GameObject.FindWithTag("CollisionManager").GetComponent<CollisionManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

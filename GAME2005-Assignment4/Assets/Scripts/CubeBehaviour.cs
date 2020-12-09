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
        StartCoroutine(waiter());
        //GameManager.collisionManager.Cubes.Add(this);

        rigidBody.velocity = new Vector3(0, !Anchored ? -1 : 0, 0);
        rigidBody.acceleration = new Vector3(0, 0, 0);
        rigidBody.mass = 5;
        rigidBody.restitution = 0.8f;
        rigidBody.friction = 0.6f;
        rigidBody.anchored = Anchored;
    }
    IEnumerator waiter()
    {
        yield return new WaitWhile(() => GameManager.collisionManager != null);

        GameManager.collisionManager.Cubes.Add(this);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    private Vector3 forwardDirection;
    private float forwardVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        forwardDirection = GameObject.FindWithTag("Player").transform.forward;
        forwardVelocity = 3;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forwardDirection * forwardVelocity * Time.deltaTime;
    }
}

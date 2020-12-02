using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 pDir;
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pDir = player.transform.forward;       
        velocity = new Vector3(0, 0, 5);
    }

    // Update is called once per frame
    void Update()
    {
       // pDir += velocity * Time.deltaTime;
        transform.position += pDir * Time.deltaTime;
    }
}

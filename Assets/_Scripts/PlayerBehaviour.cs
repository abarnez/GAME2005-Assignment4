using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public Transform bulletSpawn;
    public GameObject bullet;
    public int fireRate;


    public BulletManager bulletManager;

    [Header("Movement")] 
    public float speed;
    public bool isGrounded;

    public RigidBody3D body;
    public CubeBehaviour cube;
    public Camera playerCam;

    int lastFrame = 0;

    bool canMove = false;

    void Start()
    {
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            _Fire();
            _Move();
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene("Start");
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if(canMove)
            {
                Time.timeScale = 0;
                canMove = false;
            }
            else
            { 
                Time.timeScale = 1;
                canMove = true;
            }
        }
    }
    float speedScalar = 0.0001f;
    private void _Move()
    {
        if (Input.GetAxisRaw("Horizontal") > 0.0f)
        {
            // move right
            body.velocity += playerCam.transform.right * (isGrounded ? (speed) : (speed * speedScalar)) * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Horizontal") < 0.0f)
        {
            // move left
            body.velocity += -playerCam.transform.right * (isGrounded ? (speed) : (speed * speedScalar)) * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Vertical") > 0.0f)
        {
            // move forward
            body.velocity += playerCam.transform.forward * (isGrounded ? (speed) : (speed * speedScalar)) * Time.deltaTime;
        }

        if (Input.GetAxisRaw("Vertical") < 0.0f)
        {
            // move Back
            body.velocity += -playerCam.transform.forward * (isGrounded ? (speed) : (speed * speedScalar)) * Time.deltaTime;
        }
        if (isGrounded)
        {
            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, 0.9f);
            body.velocity = new Vector3(body.velocity.x, 0.0f, body.velocity.z); // remove y
            if (Input.GetAxisRaw("Jump") > 0.0f)
            {
                body.velocity += transform.up * speed * 0.05f * Time.deltaTime;
            }
        }
        transform.position += body.velocity;
    }


    private void _Fire()
    {
        if (Input.GetAxisRaw("Fire1") > 0.0f)
        {
            // delays firing
            if (Time.frameCount - lastFrame >= fireRate)
            {
                var tempBullet = bulletManager.GetBullet(bulletSpawn.position, bulletSpawn.forward);
                tempBullet.transform.SetParent(bulletManager.gameObject.transform);
                lastFrame = Time.frameCount;
            }
        }
    }

    void FixedUpdate()
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        isGrounded = cube.isGrounded;
    }

}

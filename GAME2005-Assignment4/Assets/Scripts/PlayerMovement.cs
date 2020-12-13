using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class PlayerMovement : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public GameObject Sliders;

    public GameObject Ball;
    public Transform FiringOrigin;
    public Transform[] Walls;

    private int lastFrame;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    float maxX = 0, minX = 1000, minZ = 1000, maxZ = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Sliders.SetActive(false);
        lastFrame = -250;

        // Get all walls min and max X and Z to lock movement.
        foreach(var wall in Walls)
        {
            if (wall.transform.position.x <= minX)
                minX = wall.transform.position.x + 0.5f;
            if(wall.transform.position.z <= minZ)
                minZ = wall.transform.position.z + 0.5f;
            if (wall.transform.position.z >= maxX)
                maxX = wall.transform.position.x - 0.5f;
            if (wall.transform.position.z >= maxZ)
                maxZ = wall.transform.position.z - 0.5f;
        }
        Debug.Log("MinX: " + minX + " maxX: " + maxX + " minZ: " + minZ + " maxZ: " + maxZ);
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller

        // Locking movement... Check if next position is > max or < min and if so dont move in that direction.
        Vector3 moveDir = (moveDirection * Time.deltaTime);
        Vector3 nextPos = characterController.transform.position + moveDir;

        if(nextPos.x <= minX || nextPos.x >= maxX)
        {
            moveDir.x = 0;
        }
        if(nextPos.z <= minZ || nextPos.z >= maxZ)
        {
            moveDir.z = 0;
        }

        characterController.Move(moveDir);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (Input.GetKeyDown("m"))
        {
            if (Sliders.activeSelf == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                Cursor.visible = false;
                Sliders.SetActive(false);
                canMove = true;
                //gui.GetComponent<Canvas>().enabled = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                Cursor.visible = true;
                Sliders.SetActive(true);
                canMove = false;
                //gui.GetComponent<Canvas>().enabled = true;
            }
        }

        if (Input.GetKeyDown("b"))
        {
            SceneManager.LoadScene("Start");
        }

        if (Input.GetMouseButtonDown(0) && canMove)
        {
            if (Time.frameCount - lastFrame >= 250)
            {
                GameObject.Instantiate(Ball, FiringOrigin.position, FiringOrigin.transform.parent.rotation);
                lastFrame = Time.frameCount;
            }
        }
    }
}
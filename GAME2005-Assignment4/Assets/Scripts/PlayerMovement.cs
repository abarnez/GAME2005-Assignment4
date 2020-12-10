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

    private int lastFrame;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    public bool backUp;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Sliders.SetActive(false);
        lastFrame = -250;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        Debug.Log(Input.GetAxis("Vertical"));
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if(Input.GetAxis("Vertical") == -1.0f) 
        {
            backUp = true;
            canMove = true;
        }
        else
        {
            backUp = false;
        }

        /*
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        */
        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

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
                GameObject.Instantiate(Ball, FiringOrigin.position, FiringOrigin.rotation);
                lastFrame = Time.frameCount;
            }
        }
    }
}
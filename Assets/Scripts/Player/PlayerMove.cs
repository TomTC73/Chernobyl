using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] //makes sure that the GameObject has a CharacterController component attached
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera; // Reference to the player's camera
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>(); // Get the CharacterController component attached to the GameObject
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward); // Get the forward direction of the player in world space
        Vector3 right = transform.TransformDirection(Vector3.right); // Get the right direction of the player in world space

        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Check if the player is holding down the Left Shift key to determine if they are running
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0; // Calculate the current speed in the forward direction based on input and whether the player is running or walking
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; // Calculate the current speed in the right direction based on input and whether the player is running or walking
        float movementDirectionY = moveDirection.y; // Store the current vertical movement direction (used for jumping and gravity)
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); // Calculate the overall movement direction by combining the forward and right movement based on input

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower; // Set the vertical movement direction to the jump power
        }
        else
        {
            moveDirection.y = movementDirectionY; // Keep the current vertical movement direction
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime; // Apply gravity to the vertical movement direction if the player is not grounded
        }

        if (Input.GetKey(KeyCode.C) && canMove) // Check if the player is holding down the C key to toggle crouching
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;

        }
        else
        {
            characterController.height = defaultHeight; // Reset the character controller height to the default height when not crouching
            walkSpeed = 6f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime); // Move the character controller based on the calculated movement direction and time delta

        if (canMove) // Check if the player can move before allowing them to look around
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; // Update the vertical rotation based on mouse input and look speed
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // Clamp the vertical rotation to prevent the player from looking too far up or down
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Rotate the player's camera based on the vertical rotation
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // Rotate the player horizontally based on mouse input and look speed
        }
    }
}
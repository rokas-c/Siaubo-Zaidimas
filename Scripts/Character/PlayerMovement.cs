using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private bool canMove = true;

    private float originalWalkSpeed;
    private float originalRunSpeed;

    // Stamina system
    public float maxStamina = 5f;  // Max running time in seconds
    private float stamina;
    public float staminaRegenRate = 1f; // Stamina recovery per second
    private bool isRunning = false;

    private float verticalVelocity = 0f;  // For gravity effects

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Store original speed values
        originalWalkSpeed = walkSpeed;
        originalRunSpeed = runSpeed;

        // Set stamina to full
        stamina = maxStamina;
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Running logic with stamina
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        if (wantsToRun && stamina > 0)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Gravity logic
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravity * Time.deltaTime;  // Apply gravity if not grounded
        }
        else
        {
            verticalVelocity = -0.5f;  // A small value to prevent the character from floating slightly
        }

        moveDirection.y = verticalVelocity;

        // Stamina management
        if (isRunning)
        {
            stamina -= Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
        else if (stamina < maxStamina)
        {
            stamina += Time.deltaTime * staminaRegenRate;
            if (stamina > maxStamina) stamina = maxStamina;
        }

        // Prevent running if out of stamina
        if (stamina <= 0)
        {
            isRunning = false;
        }

        // Move the character
        characterController.Move(moveDirection * Time.deltaTime);

        // Camera movement
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}

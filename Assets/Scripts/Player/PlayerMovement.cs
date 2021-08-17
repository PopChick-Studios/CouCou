using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;

    private CharacterController controller;
    public Transform cam;

    private float turnSmoothVelocity;

    private float moveSpeed;
    private float runTimer;
    [SerializeField, Header("Movement and Rotation Speed")] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSmoothTime;

    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;
    private bool isGrounded;
    private bool isCrouching;

    // Inputs
    PlayerInputActions playerInputActions;
    private Vector3 inputMovement;

    private void Awake()
    {
        controller = gameObject.GetComponent<CharacterController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Movement.performed += x => inputMovement = x.ReadValue<Vector2>();
        playerInputActions.Wandering.Movement.canceled += x => inputMovement = Vector2.zero;

        playerInputActions.Wandering.CrouchStart.performed += x => CrouchPressed();
        playerInputActions.Wandering.CrouchFinish.performed += x => CrouchReleased();
    }

    #region - Crouch -

    private void CrouchPressed()
    {
        isCrouching = true;
    }

    private void CrouchReleased()
    {
        isCrouching = false;
    }

    #endregion

    private void Update()
    {
        // Only move during wandering phase
        if (gameManager.State == GameManager.GameState.Wandering)
        {
            // Check whether or not to use gravity
            isGrounded = controller.isGrounded;
            if (isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            if (inputMovement.magnitude >= 0.1f)
            {
                // Choose the right movement speed
                if (isCrouching)
                {
                    moveSpeed = walkSpeed;
                }
                else if (inputMovement.magnitude != 1)
                {
                    moveSpeed = runSpeed * inputMovement.magnitude;
                    runTimer = 2;
                }
                else if (runTimer < 0)
                {
                    moveSpeed = sprintSpeed;
                }
                else
                {
                    moveSpeed = runSpeed;
                    runTimer -= Time.deltaTime;
                }

                // Create the angles
                float targetAngle = Mathf.Atan2(inputMovement.x, inputMovement.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);

                // Rotate the player
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Move the player
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
            }
            else
            {
                runTimer = 2;
            }

            // Apply gravity
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}
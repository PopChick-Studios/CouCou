using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private GameManager gameManager;
    private Player player;
    public QuestScriptable questScriptable;
    //private TerrainDetector terrainDetector;

    private CharacterController controller;
    public Animator playerAnimator;
    public Animator crossfadeAnimator;
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
    public bool canMove;

    // Inputs
    PlayerInputActions playerInputActions;
    private Vector3 inputMovement;

    private void Awake()
    {
        player = GetComponent<Player>();

        //terrainDetector = new TerrainDetector();
        controller = gameObject.GetComponent<CharacterController>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.Movement.performed += x => inputMovement = x.ReadValue<Vector2>();
        playerInputActions.Wandering.Movement.canceled += x => inputMovement = Vector2.zero;

        playerInputActions.Wandering.CrouchStart.performed += x => CrouchPressed();
        playerInputActions.Wandering.CrouchFinish.performed += x => CrouchReleased();
    }

    private void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if (data == null)
        {
            return;
        }
        player.questProgress = data.questProgress;
        player.amountOfCapsules = data.amountOfCapsules;
        controller.enabled = false;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        cam.position = transform.position;
        questScriptable.questProgress = data.questProgress;
        questScriptable.subquestProgress = data.subquestProgress;
        controller.enabled = true;
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
        if (gameManager.State == GameManager.GameState.Wandering && canMove)
        {
            // Check whether or not to use gravity
            isGrounded = controller.isGrounded;
            if (isGrounded && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            if (inputMovement.magnitude >= 0.1f)
            {
                playerAnimator.SetBool("isWalking", true);
                // Choose the right movement speed
                if (isCrouching)
                {
                    playerAnimator.speed = 1;
                    playerAnimator.SetBool("isCrouching", true);
                    moveSpeed = walkSpeed;
                }
                else if (inputMovement.magnitude < 0.9)
                {
                    Debug.Log(inputMovement.magnitude);
                    playerAnimator.SetBool("isCrouching", false);
                    playerAnimator.SetBool("isRunning", false);
                    runTimer = 2;
                    moveSpeed = runSpeed * inputMovement.magnitude;
                }
                else if (runTimer < 0)
                {
                    playerAnimator.SetBool("isRunning", true);
                    moveSpeed = sprintSpeed * Mathf.Clamp(Mathf.Abs(runTimer) / 2 + 1, 1, 2.5f);
                    playerAnimator.speed = Mathf.Clamp(Mathf.Abs(runTimer) / 5 + 1, 1, 1.25f);
                    runTimer -= Time.deltaTime;
                }
                else
                {
                    playerAnimator.speed = 1;
                    playerAnimator.SetBool("isCrouching", false);
                    playerAnimator.SetBool("isRunning", false);
                    moveSpeed = runSpeed * Mathf.Clamp(Mathf.Abs(2 - runTimer) + 1, 1, 2.5f);
                    playerAnimator.speed = Mathf.Clamp(Mathf.Abs(runTimer) / 2, 1, 1.25f);
                    runTimer -= Time.deltaTime;
                }

                // Create the angles
                float targetAngle = Mathf.Atan2(inputMovement.x, inputMovement.y) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, rotationSmoothTime);

                // Rotate the player
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Move the player
                Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveSpeed * Time.deltaTime * moveDirection.normalized);

                //Debug.Log(terrainDetector.GetActiveTerrainTextureIdx(transform.position));
            }
            else
            {
                playerAnimator.SetBool("isCrouching", false);
                playerAnimator.SetBool("isRunning", false);
                playerAnimator.SetBool("isWalking", false);
                runTimer = 2;
            }

            // Apply gravity
            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
        else if (!canMove)
        {
            playerAnimator.SetBool("isCrouching", false);
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isWalking", false);
            runTimer = 2;
        }
    }

    public IEnumerator WarpPlayer(Vector3 position)
    {
        crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);
        canMove = false;
        controller.enabled = false;
        transform.position = position;
        cam.position = position;
        controller.enabled = true;
        crossfadeAnimator.SetTrigger("Reset");
        yield return new WaitForSeconds(1f);
        canMove = true;
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
